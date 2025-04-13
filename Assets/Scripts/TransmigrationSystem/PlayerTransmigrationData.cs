using UnityEngine;

namespace TransmigrationSystem
{
    public class PlayerTransmigrationData
    {
        [SerializeField] private GameObject _playerObject;
        [SerializeField] private GameObject _activeObject = null;

        public GameObject PlayerObject
        {
            get => _playerObject;
        }

        public GameObject ActiveObject
        {
            get => _activeObject;
            set
            {
                _activeObject = value;
                _playerObject.SetActive(false);
                _activeObject.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
        
        public void ReturnToCharacter()
        {
            _activeObject.GetComponent<Rigidbody>().isKinematic = true;
            _playerObject.transform.position = _activeObject.transform.position;
            
            _playerObject.SetActive(true);
            _playerObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 3);
            
        }
    }
}