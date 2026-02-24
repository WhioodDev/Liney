using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] private float tickTime;
    [SerializeField] private int maxTailSize;
    [SerializeField] private Transform tailParent;
    private float tickTimer;
    private Vector2 _movementVector;
    private Vector2 _currentDirection;
    private List<GameObject> _tailBlocks;
    private GameObject _squarePrefab;
    private SpriteRenderer _spriteRenderer;
    private Vector2 _queuedDirection;

    private void Start()
    {
        _tailBlocks = new List<GameObject>();
        _squarePrefab = Resources.Load<GameObject>("Square");
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _currentDirection = Vector2.left;
        _queuedDirection = _currentDirection;
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        _movementVector = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        tickTimer += Time.deltaTime;
        UpdateCurrentDirection();
        if (tickTimer >= tickTime)
            MovementTick();
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
        tickTimer = 0;

        // apply queued input once per tick
        _currentDirection = _queuedDirection;

        GameObject newTailblock = Instantiate(
            _squarePrefab,
            transform.position,
            Quaternion.identity,
            tailParent
        );

        newTailblock.transform.localScale = transform.localScale;
        _tailBlocks.Add(newTailblock);

        DeleteLongTail();

        transform.position = new Vector2(
            transform.position.x + _currentDirection.x * _spriteRenderer.bounds.size.x,
            transform.position.y + _currentDirection.y * _spriteRenderer.bounds.size.y
        );
    }

    private void DeleteLongTail()
    {
        if (_tailBlocks.Count > maxTailSize)
        {
            GameObject tailBlockToRemove = _tailBlocks[0];
            _tailBlocks.Remove(tailBlockToRemove);
            Destroy(tailBlockToRemove);
        }
    }
}
