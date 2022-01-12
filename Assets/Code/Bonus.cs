using PlayerNS;
using UnityEngine;
using UnityEngine.Networking;

public class Bonus : NetworkBehaviour
{
    #region privateFields

    [SerializeField] private float _rotationSpeed;

    #endregion


    #region privateMethods
    private void Update()
    {
        transform.Rotate(0, _rotationSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //other.GetComponent<Player>();
            other.GetComponent<Player>().IncreasePoints();
            CmdDestroyThisNetworkObject();
        }
    }

    [Command]
    private void CmdDestroyThisNetworkObject()
    {
        RpcDestroyThisNetworkObject();
    }

    [ClientRpc]
    public void RpcDestroyThisNetworkObject()
    {
        Destroy(gameObject);
    }

    #endregion
}
