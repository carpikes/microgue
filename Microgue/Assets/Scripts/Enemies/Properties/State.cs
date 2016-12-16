public interface State<Enemy>
{
    void Update(Enemy owner);
    void FixedUpdate(Enemy owner);
    void OnEnter(Enemy owner);
    void OnExit(Enemy owner);
}