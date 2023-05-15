﻿using System.Linq;
using General;
using General.Damageable;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VR
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class BaseGun : MonoBehaviour
    {
        private XRGrabInteractable _interactable;
        [SerializeField] private float _bulletSpeed = 1000.0f;
        [SerializeField] private float _bulletDrop = 0.0f;

        [SerializeField] private ParticleSystem _muzzleParticles;
        [SerializeField] private ParticleSystem _impactParticles;
        [SerializeField] private Transform _raycastOrigin;

        private Ray _ray;
        private RaycastHit _hit;
        [SerializeField] private LayerMask _layerMask;

        [SerializeField] private int currentBullets = 6;
        [SerializeField] private TMP_Text _bulletsText;
        [SerializeField] private GameObject _bulletPrefab;

        [SerializeField] private Transform _orientation;
        [SerializeField] private ObjectPooling _bulletsPooling;

        [SerializeField] private bool isShopGun;

        [SerializeField] private bool gunBought;
        [SerializeField] private int price;
        [SerializeField] private Vector3 startPos;
        [SerializeField] private Quaternion startRot;

        private void Start()
        {
            _interactable = GetComponent<XRGrabInteractable>();
            _bulletsText = GetComponentInChildren<TMP_Text>();

            if (GetComponentInParent<ShopInstance>() != null)
            {
                isShopGun = true;
                price = GetComponentInParent<ShopInstance>().ShopItem.GetPrice;
            }

            _interactable.selectEntered.AddListener(SelectEnter);
            _interactable.selectExited.AddListener(SelectExit);
            _interactable.hoverEntered.AddListener(HoverEnter);
            _interactable.hoverExited.AddListener(HoverExit);
            _interactable.activated.AddListener(PerformShoot);

            startPos = transform.localPosition;
            startRot = transform.localRotation;

            _bulletsPooling =
                GameManager.Instance.objectPoolingManager.GetNewObjectPool("RevolverVRBullets", ref _bulletPrefab, 5);
        }

        private void SelectEnter(SelectEnterEventArgs args)
        {
            if (isShopGun)
                GetGun();
            else
                BuyGun();
        }

        private void SelectExit(SelectExitEventArgs args)
        {
            if (isShopGun)
                if (gunBought)
                    SendGunToHand();
                else
                    ResetGunPos();
            else
                ResetGunPos();
        }


        private void HoverEnter(HoverEnterEventArgs args)
        {
            if (gunBought) return;

            if (isShopGun)
            {
                SetInteractableLayerBasedOnPrice();
            }
            else
                UnableInteractableLayer();
        }

        private void HoverExit(HoverExitEventArgs args)
        {
            if (gunBought) return;
            ResetInteractableLayer();
        }

        #region SelectEnter

        protected virtual void GetGun()
        {
            _orientation.rotation = Quaternion.Euler(0, -90, 0);
        }

        protected virtual void BuyGun()
        {
            if (GameManager.Instance.players.First().PlayerData._economy >= price)
            {
                GameManager.Instance.players.First().PlayerData.Buy(price);
                gunBought = true;
            }
        }

        #endregion

        #region SelectExit

        protected virtual void SendGunToHand()
        {
            _interactable.interactionLayers = 2;
            GetComponent<Rigidbody>().isKinematic = false;
            transform.parent = null;
        }

        protected virtual void ResetGunPos()
        {
            transform.localPosition = startPos;
            transform.localRotation = startRot;
        }

        #endregion

        #region HoverEnter

        protected virtual void SetInteractableLayerBasedOnPrice()
        {
            var currentEconomy = GameManager.Instance.players.First().PlayerData._economy;
            var currentPrice = GetComponentInParent<ShopInstance>().ShopItem.GetPrice;
            _interactable.interactionLayers = currentEconomy >= currentPrice ? 2 : 0;   
        }

        protected virtual void UnableInteractableLayer()
        {
            _interactable.interactionLayers = 0;
        }

        #endregion

        #region HoverExit

        protected virtual void ResetInteractableLayer()
        {
            _interactable.interactionLayers = 2;
        }
        #endregion

        private void Update()
        {
            MoveBulletVR();
            DeleteBullets();
        }

        private void DeleteBullets()
        {
            _bulletsPooling.GetPool().ForEach(
                bullet =>
                {
                    if (!bullet.activeSelf) return;
                    BulletVR bvr = bullet.GetComponent<BulletVR>();
                    if (bvr._time >= bvr._waitTime)
                        bullet.SetActive(false);
                }
            );
        }

        private void PerformShoot(ActivateEventArgs args) => Shoot();

        private Vector3 GetBulletWorldPosition(BulletVR bullet)
        {
            //p + v*t + 0.5*g*t*t
            Vector3 gravity = Vector3.down * _bulletDrop;
            return bullet._initialPos + bullet._initialVel * bullet._time +
                   gravity * (0.5f * bullet._time * bullet._time);
        }

        private void MoveBulletVR()
        {
            if (_bulletsPooling == null) return;
            _bulletsPooling.GetPool().ForEach(bullet =>
            {
                if (!bullet.activeSelf) return;

                var bulletVR = bullet.GetComponent<BulletVR>();
                Vector3 pos0 = GetBulletWorldPosition(bulletVR);
                bulletVR._time += Time.deltaTime;
                Vector3 pos1 = GetBulletWorldPosition(bulletVR);
                RayCastBulletSegment(pos0, pos1, bulletVR);
            });
        }

        private void RayCastBulletSegment(Vector3 start, Vector3 end, BulletVR bullet)
        {
            Vector3 direction = end - start;
            _ray.origin = start;
            _ray.direction = direction;

            if (Physics.Raycast(_ray, out _hit, direction.magnitude, _layerMask))
            {
                bullet._trail.transform.position = _hit.point;
                bullet._time = 3f;
                if (!CheckForForce(_hit.transform.gameObject, _hit.point, _ray.direction))
                {
                    var trf = _impactParticles.transform;
                    trf.position = _hit.point;
                    trf.forward = _hit.normal;
                    // _impactParticles.transform.SetParent(_hit.transform);
                    _impactParticles.Emit(1);
                }

                CheckForDamage(_hit.transform.gameObject);
            }
            else
            {
                bullet._trail.transform.position = end;
            }
        }

        private void Shoot()
        {
            Vector3 velocity = _raycastOrigin.forward.normalized * _bulletSpeed;

            BulletVR bullet = _bulletsPooling.GetPooledElement().GetComponent<BulletVR>();
            bullet.enabled = true;
            bullet.Init(_raycastOrigin.position, velocity);

            currentBullets--;
            UpdateText();
            _muzzleParticles.Emit(1);
        }

        private bool CheckForForce(GameObject hitObject, Vector3 hitPosition, Vector3 direction)
        {
            bool hasRb = false;
            if (hitObject.TryGetComponent(out Rigidbody rb))
            {
                rb.AddExplosionForce(1500f, hitPosition, 0.2f);
                hasRb = true;
            }

            return hasRb;
        }

        private void CheckForDamage(GameObject hitObject)
        {
            if (hitObject.TryGetComponent(out Damageable damageable))
                damageable.Damage(this);
        }

        private void UpdateText() => _bulletsText.text = currentBullets.ToString();

        public void CallShootFromDebugger()
        {
            PerformShoot(null);
        }

        public enum ShootType
        {
            Single,
            Burst,
            Auto
        }
    }
}