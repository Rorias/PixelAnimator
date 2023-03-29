public sealed class test3
{
#region Singleton
private static test3 _instance;
private static object _lock = new object();

private test3() { }

public static test3 Instance
{
get
{
if (null == _instance)
{
lock (_lock)
{
if (null == _instance)
{
_instance = new test3();
}
}
}
return _instance;
}
}
#endregion

public AnimationStateMachine asm;

public void test3(int subFrame)
{
switch(subFrame)
{
case 0:
asm.SetPart(0, 1, -1.53125f, 0.8125f, false);
asm.SetPart(1, 17, -0.53125f, -1.125f, false);
asm.SetPart(2, 42, -0.0625f, 0.3125f, false);
asm.SetPart(3, 65, 0f, 1.65625f, false);
asm.SetPart(4, 89, -0.25f, 2.34375f, false);
asm.SetPart(5, 104, 1.0625f, -1.34375f, false);
asm.SetPart(6, 136, 1.28125f, 0.78125f, false);
break;
case 1:
asm.SetPart(0, 1, -1.15625f, 0.0625f, false);
asm.SetPart(1, 17, -0.84375f, -1.5f, false);
asm.SetPart(2, 40, -0.09375f, -0.125f, false);
asm.SetPart(3, 55, -0.125f, 1.15625f, false);
asm.SetPart(4, 83, -0.0625f, 1.78125f, false);
asm.SetPart(5, 101, 0.46875f, -1.21875f, false);
asm.SetPart(6, 117, 1f, -0.03125f, false);
break;
case 2:
asm.SetPart(0, 6, -0.78125f, -0.34375f, false);
asm.SetPart(1, 25, -0.03125f, -0.625f, false);
asm.SetPart(2, 31, 0.40625f, 0.0625f, false);
asm.SetPart(3, 57, -0.03125f, 0.90625f, false);
asm.SetPart(4, 84, 0.125f, 1.625f, false);
asm.SetPart(5, 107, 1.09375f, -0.75f, false);
asm.SetPart(6, 113, 0.34375f, -1.21875f, false);
break;
}
}
}