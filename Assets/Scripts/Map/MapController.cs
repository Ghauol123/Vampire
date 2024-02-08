using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> terrianChunks;
    [SerializeField]
    private GameObject player;
    private float checkerRadius;
    private Vector3 noTerrianPosition;
    [HideInInspector]
    public GameObject currentChunk;
    [SerializeField]
    private LayerMask terrianMask;
    private PlayerMoving _playerMoving;
    [Header("Optimization")]
    [SerializeField]
    private List<GameObject> spawnChunks;
    [SerializeField]
    private GameObject _lastChunks;
    [SerializeField]
    private float maxOpDist;
    private float opDist;
    private float optimizerCoolDown;
    [SerializeField]
    private float optimizerCooldownDur;
    // Start is called before the first frame update
    void Start()
    {
        _playerMoving = FindObjectOfType<PlayerMoving>();
    }

    // Update is called once per frame
    void Update()
    {
        ChunkChecker();
        ChunkOptimization();
    }
    void ChunkChecker()
    {
        if (!currentChunk)
        {
            return;
        }
        if (_playerMoving.moveDir.x < 0 && _playerMoving.moveDir.y < 0)
        { // left down
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Left Down").position, checkerRadius, terrianMask))
            {
                noTerrianPosition = currentChunk.transform.Find("Left Down").position;
                SpawnChunk();
            }
        }
        if (_playerMoving.moveDir.x < 0 && _playerMoving.moveDir.y > 0)
        { // left up
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Left Up").position, checkerRadius, terrianMask))
            {
                noTerrianPosition = currentChunk.transform.Find("Left Up").position;
                SpawnChunk();
            }
        }
        if (_playerMoving.moveDir.x > 0 && _playerMoving.moveDir.y < 0)
        {// right down
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Right Down").position, checkerRadius, terrianMask))
            {
                noTerrianPosition = currentChunk.transform.Find("Right Down").position;
                SpawnChunk();
            }
        }
        if (_playerMoving.moveDir.x > 0 && _playerMoving.moveDir.y > 0)
        {// right up
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Right Up").position, checkerRadius, terrianMask))
            {
                noTerrianPosition = currentChunk.transform.Find("Right Up").position;
                SpawnChunk();
            }
        }
        if (_playerMoving.moveDir.x == 0 && _playerMoving.moveDir.y > 0)
        {// up
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Up").position, checkerRadius, terrianMask))
            {
                noTerrianPosition = currentChunk.transform.Find("Up").position;
                SpawnChunk();
            }
        }
        if (_playerMoving.moveDir.x == 0 && _playerMoving.moveDir.y < 0)
        {// down
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Down").position, checkerRadius, terrianMask))
            {
                noTerrianPosition = currentChunk.transform.Find("Down").position;
                SpawnChunk();
            }
        }
        if (_playerMoving.moveDir.x > 0 && _playerMoving.moveDir.y == 0)
        {// right
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Right").position, checkerRadius, terrianMask))
            {
                noTerrianPosition = currentChunk.transform.Find("Right").position;
                SpawnChunk();
            }
        }
        if (_playerMoving.moveDir.x < 0 && _playerMoving.moveDir.y == 0)
        {// left
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Left").position, checkerRadius, terrianMask))
            {
                noTerrianPosition = currentChunk.transform.Find("Left").position;
                SpawnChunk();
            }
        }
    }
    void SpawnChunk()
    {
        int ran = UnityEngine.Random.Range(0, terrianChunks.Count);
        _lastChunks = Instantiate(terrianChunks[ran], noTerrianPosition, UnityEngine.Quaternion.identity);
        spawnChunks.Add(_lastChunks);
    }
    public void ChunkOptimization(){
        optimizerCoolDown -= Time.deltaTime;
        if(optimizerCoolDown <= 0){
            optimizerCoolDown = optimizerCooldownDur;
        }
        else{
            return;
        }
        foreach(GameObject chunk in spawnChunks){
            opDist = Vector3.Distance(player.transform.position,chunk.transform.position);
            if(opDist > maxOpDist){
                chunk.SetActive(false);
            }
            else{
                chunk.SetActive(true);
            }
        }
    }
}
