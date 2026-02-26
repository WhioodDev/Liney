using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxTailSize;
    [SerializeField] private float whiteAmountToLose;
    
    [Space(5), Header("References")]
    [SerializeField] private Transform tailParent;
    [SerializeField] private GameManager gameManager;
    
    private GameObject _squarePrefab;
    private Vector2 _movementVector;
    private Vector2 _currentDirection;
    private List<GameObject> _tailBlocks;
    private SpriteRenderer _spriteRenderer;
    private Vector2 _queuedDirection;
    private Vector2 _lastStartPosition;
    private bool _canMove = false;

    private void Awake()
    {
        MapManager.OnMapChange += OnMapChange;
        GameManager.OnTick += MovementTick;
    }

    private void Start()
    {
        _tailBlocks = new List<GameObject>();
        _squarePrefab = Resources.Load<GameObject>("Square");
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _queuedDirection = _currentDirection;
        transform.localScale = gameManager.GetTileSize();
    }

    private void OnDisable()
    {
        MapManager.OnMapChange -= OnMapChange;
        GameManager.OnTick -= MovementTick;
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        _movementVector = context.ReadValue<Vector2>();
    }
    
    private void Update()
    {
        if (!_canMove) return;
        UpdateCurrentDirection();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<IDeadly>(out IDeadly deadly))
        {
            ResetPlayer();
        }

        if (other.gameObject.TryGetComponent<IFinish>(out IFinish finish))
        {
            gameManager.Finish();
        }
    }
    
    private void OnMapChange(Vector2 startPos, float tickSpeed)
    {
        _lastStartPosition = startPos;
        transform.position = _lastStartPosition;
        _canMove = true;
        transform.localScale = gameManager.GetTileSize();
        ResetPlayer();
    }

    private bool IsOpposite(Vector2 a, Vector2 b)
    {
        return a + b == Vector2.zero;
    }
    
    private void UpdateCurrentDirection()
    {
        if (_movementVector == Vector2.zero)
            return;

        Vector2 newDirection;

        // prevent diagonals
        if (Mathf.Abs(_movementVector.x) > Mathf.Abs(_movementVector.y))
            newDirection = new Vector2(Mathf.Sign(_movementVector.x), 0);
        else
            newDirection = new Vector2(0, Mathf.Sign(_movementVector.y));

        // IMPORTANT:
        // compare against CURRENT movement direction,
        // not queued direction
        if (!IsOpposite(newDirection, _currentDirection))
            _queuedDirection = newDirection;
    }

    private void MovementTick()
    {
        // apply queued input once per tick
        _currentDirection = _queuedDirection;

        GameObject newTailblock = Instantiate(
            _squarePrefab,
            transform.position,
            Quaternion.identity,
            tailParent
        );

        newTailblock.transform.localScale = transform.localScale;
        newTailblock.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
        _tailBlocks.Add(newTailblock);

        DeleteLongTail(false);

        transform.position = new Vector2(
            transform.position.x + _currentDirection.x * _spriteRenderer.bounds.size.x,
            transform.position.y + _currentDirection.y * _spriteRenderer.bounds.size.y
        );
        
        GradientTail();
    }

    private void DeleteLongTail(bool removeAll)
    {
        if (_tailBlocks == null) return;
        if (removeAll)
        {
            for(int i = tailParent.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(tailParent.transform.GetChild(i).gameObject);
            }
            _tailBlocks.Clear();
        }
        if (_tailBlocks.Count > maxTailSize)
        {
            GameObject tailBlockToRemove = _tailBlocks[0];
            _tailBlocks.Remove(tailBlockToRemove);
            Destroy(tailBlockToRemove);
        }
    }    

    private void GradientTail()
    {
        float whiteAmount = 1 - _tailBlocks.Count * whiteAmountToLose;
        foreach (GameObject tailBlock in _tailBlocks)
        {
            whiteAmount += 0.05f;
            tailBlock.GetComponent<SpriteRenderer>().color = new Color(whiteAmount, whiteAmount, whiteAmount, whiteAmount);
        }
    }



    private void ResetPlayer()
    {
        transform.position = _lastStartPosition;
        _currentDirection = Vector2.zero;
        _queuedDirection = Vector2.zero;
        DeleteLongTail(true);
    }
}
