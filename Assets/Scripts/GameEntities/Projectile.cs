using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float _baseDamage;
    private GameObject _explosion;
    private Team _team;
    private Vector2 _direction;
    private float _force;
    private Rigidbody2D _rigidbody;
    private Transform _transform;

    public void Init(float baseDamage, GameObject explosion, Team team, Vector2 direction, float force)
    {
        _baseDamage = baseDamage;
        _explosion = explosion;
        _team = team;
        _direction = direction;
        _force = force;
        _rigidbody = GetComponent<Rigidbody2D>();

        ApplyTeam();
        ApplyForce();
    }

    void Update()
    {
        _transform = transform;
        DestroyWhenOutsideTheMap();

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //TODO: Apply damage
        Instantiate(_explosion, transform.position, transform.rotation);
        GameObject.Destroy(gameObject);
    }

    void ApplyForce()
    {
        _rigidbody.velocity = _direction * _force;
    }
    void ApplyTeam()
    {
        gameObject.layer = _team.GetProjectileTeamLayer();
    }

    void DestroyWhenOutsideTheMap()
    {
        if (_transform.position.x > GameState.Instance.CurrentMap.TopRight.x
        || _transform.position.y > GameState.Instance.CurrentMap.TopRight.y
        || _transform.position.x < GameState.Instance.CurrentMap.BottomLeft.x
        || _transform.position.y < GameState.Instance.CurrentMap.BottomLeft.y)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
