using UnityEngine;
using UnityEngine.InputSystem;

public class Hook : MonoBehaviour
{
    [SerializeField] private GameObject _impactPrefab;
    [SerializeField] private GameObject  _shootingEffectPrefab;
    [SerializeField] private float _hookForce;
    [SerializeField] private float _hookPullOffset;
    [SerializeField] private float _cameraShake;
    [SerializeField] private float _cameraShakeDuration;
    private Player _player;
    private Rigidbody2D _rigidbody;
    private Rigidbody2D _playerRigidbody;
    private float _originalPullOffset;
    private float _offsetChanger;
    private Gamepad _gamepad;
    private Transform _playerTransform;
    private Transform _transform;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _player = gameObject.GetComponentInParent<Player>();
        _playerRigidbody = _player.GetComponent<Rigidbody2D>();
        _originalPullOffset = _hookPullOffset;
        _gamepad = _player.GetComponent<PlayerInput>().GetDevice<Gamepad>();
    }

    float gambiSoundTimer = 0; // TODO: PLEASE MOVE THIS TO SOUND SYSTEM, it should have an option to only play the sound after some time
    // Update is called once per frame
    void Update()
    {
        // Cache transform
        _playerTransform = _player.transform;
        _transform = transform;

        if (gambiSoundTimer > 1f)
            gambiSoundTimer = 0; 
        else if(gambiSoundTimer > 0)
            gambiSoundTimer += Time.deltaTime;

        RenderRope();
        if(_rigidbody.bodyType == RigidbodyType2D.Static)
        {
            Vector2 fromPlayerToHook = (Vector2)_transform.position - (Vector2)_playerTransform.position;
            fromPlayerToHook.Normalize();
            float distance = Vector2.Distance(_transform.position, _playerTransform.position);

            if((_hookPullOffset > _originalPullOffset && _offsetChanger < 0) || (_hookPullOffset < _originalPullOffset * 6 && _offsetChanger > 0))
            {
                _hookPullOffset += _offsetChanger * Time.deltaTime * 10;

                if(gambiSoundTimer == 0) // TODO: PLEASE MOVE THIS TO SOUND SYSTEM, it should have an option to only play the sound after some time
                {
                    GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayElastic();// TODO: PLEASE MOVE THIS TO SOUND SYSTEM, it should have an option to only play the sound after some time
                    gambiSoundTimer += Time.deltaTime;
                }
            }




            if (distance > _hookPullOffset)
            {
                _playerRigidbody.velocity += fromPlayerToHook * _hookForce * Time.deltaTime;
            }

        }
    }

    void OnMove(InputValue inputValue)
    {
        _offsetChanger = inputValue.Get<Vector2>().y * -1;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Utils.GamepadRumble(_gamepad,0f,1f,0.2f);
        GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayHookHit();
        GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayElastic();
        _rigidbody.bodyType = RigidbodyType2D.Static;
        _hookPullOffset = _originalPullOffset;
        Instantiate(_impactPrefab, _transform.position, _transform.rotation);
        _player.camera.GetComponent<PlayerCamera>().StartShake(_cameraShakeDuration, _cameraShake);
    }

    void RenderRope()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        Vector3[] positions = { _transform.position,  _playerTransform.position };
        lineRenderer.SetPositions(positions);
    }
    public void PlayShootAnimataion()
    {
        Instantiate(_shootingEffectPrefab, _transform.position, _transform.rotation);
    }

}
