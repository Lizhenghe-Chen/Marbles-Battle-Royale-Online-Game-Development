using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ParticleFollow : MonoBehaviour
{
    [SerializeField] float emissionThreshold;
    //============================================================================
    [Header("Below for debug use:\n")]
    [SerializeField] Transform Player;
    [SerializeField] SphereCollider playerCollider;
    [SerializeField] float angularVelocity;
    //[SerializeField] bool enableParticleSystem;

    Rigidbody player_rg;
    [SerializeField] ParticleSystem PS;
   // PhotonView photonView;
   // [SerializeField] PlayerManager playerManager;
    void Awake()
    {
     //   photonView = transform.parent.GetComponent<PhotonView>();
    }
    void Start()
    {
        // if (!photonView.IsMine) { return; }

        Player = transform.parent;
      //  playerManager = Player.GetComponent<MovementController>().playerManager;
        playerCollider = Player.GetComponent<SphereCollider>();
        player_rg = Player.GetComponent<Rigidbody>();
        //jumpController = Player.GetComponent<JumpController>();
        PS = transform.GetComponent<ParticleSystem>();


    }

    // Update is called once per frame
    void Update()
    {
        //if (photonView.IsMine) { Emmission(); }
      //  Emmission();
        Follow();

    }
    void Emmission()
    {

        //  enableParticleSystem = playerManager.PS;
        angularVelocity = player_rg.velocity.magnitude;
        if (angularVelocity >= emissionThreshold)
        {
            PS.Play();
        }
        else { PS.Stop(); }

        // if (playerManager.PS)
        // {
        //     PS.Play();
        // }
        // else
        // {
        //     PS.Stop();
        // }
    }
    void Follow()
    {
        Vector3 target = new Vector3(Player.position.x, Player.position.y - playerCollider.radius * 0.7f, Player.position.z);
        transform.position = Vector3.MoveTowards(transform.position, target, 100);
    }
}
