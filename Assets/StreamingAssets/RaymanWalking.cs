public sealed class RaymanWalking
{
#region Singleton
private static RaymanWalking _instance;
private static object _lock = new object();

private RaymanWalking() { }

public static RaymanWalking Instance
{
get
{
if (null == _instance)
{
lock (_lock)
{
if (null == _instance)
{
_instance = new RaymanWalking();
}
}
}
return _instance;
}
}
#endregion

public AnimationStateMachine asm;

public void RaymanWalking(int subFrame)
{
switch(subFrame)
{
case 0:
asm.SetPart(0, 2, -0.125f, -0.90625f, false);
asm.SetPart(1, 16, -0.4375f, -1.09375f, false);
asm.SetPart(2, 49, -0.0625f, -0.09375f, false);
asm.SetPart(3, 53, -0.5f, 1.0625f, false);
asm.SetPart(4, 83, -0.5f, 1.75f, false);
asm.SetPart(5, 108, 0.1875f, -1.625f, false);
asm.SetPart(6, 117, -0.0625f, -1f, false);
break;
case 1:
asm.SetPart(0, 2, 0.125f, -0.90625f, false);
asm.SetPart(1, 16, -0.5625f, -1.15625f, false);
asm.SetPart(2, 49, -0.0625f, -0.09375f, false);
asm.SetPart(3, 53, -0.5f, 1.0625f, false);
asm.SetPart(4, 83, -0.5f, 1.75f, false);
asm.SetPart(5, 108, 0.3125f, -1.625f, false);
asm.SetPart(6, 117, -0.375f, -1f, false);
break;
case 2:
asm.SetPart(0, 2, 0.3125f, -0.90625f, false);
asm.SetPart(1, 16, -0.6875f, -1.21875f, false);
asm.SetPart(2, 49, -0.0625f, -0.09375f, false);
asm.SetPart(3, 53, -0.5f, 1.0625f, false);
asm.SetPart(4, 83, -0.5f, 1.75f, false);
asm.SetPart(5, 108, 0.4375f, -1.625f, false);
asm.SetPart(6, 118, -0.625f, -0.96875f, false);
break;
case 3:
asm.SetPart(0, 2, 0.5625f, -0.78125f, false);
asm.SetPart(1, 16, -0.875f, -1.21875f, false);
asm.SetPart(2, 50, 0f, -0.03125f, false);
asm.SetPart(3, 53, -0.5f, 1.0625f, false);
asm.SetPart(4, 84, -0.375f, 1.78125f, false);
asm.SetPart(5, 108, 0.5625f, -1.625f, false);
asm.SetPart(6, 118, -0.9375f, -0.84375f, false);
break;
case 4:
asm.SetPart(0, 2, 0.625f, -0.78125f, false);
asm.SetPart(1, 16, -1.0625f, -1.34375f, false);
asm.SetPart(2, 50, -0.0625f, -0.09375f, false);
asm.SetPart(3, 53, -0.5f, 1f, false);
asm.SetPart(4, 84, -0.375f, 1.71875f, false);
asm.SetPart(5, 109, 0.6875f, -1.53125f, false);
asm.SetPart(6, 112, -1.0625f, -0.75f, false);
break;
case 5:
asm.SetPart(0, 2, 0.75f, -0.71875f, false);
asm.SetPart(1, 28, -1.1875f, -1.625f, false);
asm.SetPart(2, 50, -0.0625f, -0.15625f, false);
asm.SetPart(3, 53, -0.5f, 0.9375f, false);
asm.SetPart(4, 84, -0.375f, 1.65625f, false);
asm.SetPart(5, 109, 0.8125f, -1.53125f, false);
asm.SetPart(6, 112, -1.1875f, -0.625f, false);
break;
case 6:
asm.SetPart(0, 2, 0.8125f, -0.71875f, false);
asm.SetPart(1, 28, -1.0625f, -1.625f, false);
asm.SetPart(2, 50, -0.0625f, -0.21875f, false);
asm.SetPart(3, 53, -0.5f, 0.8125f, false);
asm.SetPart(4, 84, -0.375f, 1.53125f, false);
asm.SetPart(5, 109, 0.9375f, -1.53125f, false);
asm.SetPart(6, 112, -1.25f, -0.625f, false);
break;
case 7:
asm.SetPart(0, 2, 0.9375f, -0.59375f, false);
asm.SetPart(1, 28, -0.9375f, -1.625f, false);
asm.SetPart(2, 43, 0.0625f, -0.21875f, false);
asm.SetPart(3, 53, -0.5f, 0.8125f, false);
asm.SetPart(4, 85, -0.4375f, 1.5f, false);
asm.SetPart(5, 109, 1.0625f, -1.53125f, false);
asm.SetPart(6, 112, -1.3125f, -0.4375f, false);
break;
case 8:
asm.SetPart(0, 2, 0.9375f, -0.59375f, false);
asm.SetPart(1, 28, -0.8125f, -1.625f, false);
asm.SetPart(2, 43, 0.0625f, -0.21875f, false);
asm.SetPart(3, 53, -0.5f, 0.8125f, false);
asm.SetPart(4, 85, -0.4375f, 1.5f, false);
asm.SetPart(5, 107, 0.9375f, -1.375f, false);
asm.SetPart(6, 112, -1.1875f, -0.5f, false);
break;
case 9:
asm.SetPart(0, 2, 0.875f, -0.65625f, false);
asm.SetPart(1, 28, -0.6875f, -1.625f, false);
asm.SetPart(2, 43, 0.0625f, -0.15625f, false);
asm.SetPart(3, 53, -0.5f, 0.875f, false);
asm.SetPart(4, 85, -0.4375f, 1.5625f, false);
asm.SetPart(5, 107, 0.6875f, -1.3125f, false);
asm.SetPart(6, 112, -1.0625f, -0.625f, false);
break;
case 10:
asm.SetPart(0, 2, 0.75f, -0.71875f, false);
asm.SetPart(1, 28, -0.5625f, -1.625f, false);
asm.SetPart(2, 43, 0.0625f, -0.15625f, false);
asm.SetPart(3, 53, -0.5f, 0.875f, false);
asm.SetPart(4, 85, -0.4375f, 1.5625f, false);
asm.SetPart(5, 107, 0.5625f, -1.25f, false);
asm.SetPart(6, 112, -0.9375f, -0.6875f, false);
break;
case 11:
asm.SetPart(0, 2, 0.5625f, -0.84375f, false);
asm.SetPart(1, 28, -0.4375f, -1.625f, false);
asm.SetPart(2, 44, 0.0625f, -0.21875f, false);
asm.SetPart(3, 53, -0.5f, 0.9375f, false);
asm.SetPart(4, 82, -0.5f, 1.5625f, false);
asm.SetPart(5, 107, 0.4375f, -1.25f, false);
asm.SetPart(6, 112, -0.75f, -0.8125f, false);
break;
case 12:
asm.SetPart(0, 2, 0.375f, -0.84375f, false);
asm.SetPart(1, 28, -0.3125f, -1.625f, false);
asm.SetPart(2, 44, 0.0625f, -0.21875f, false);
asm.SetPart(3, 53, -0.5f, 0.9375f, false);
asm.SetPart(4, 82, -0.5f, 1.5625f, false);
asm.SetPart(5, 107, 0.3125f, -1.3125f, false);
asm.SetPart(6, 118, -0.5625f, -0.84375f, false);
break;
case 13:
asm.SetPart(0, 2, 0.1875f, -0.84375f, false);
asm.SetPart(1, 28, -0.1875f, -1.625f, false);
asm.SetPart(2, 44, 0.0625f, -0.15625f, false);
asm.SetPart(3, 53, -0.5f, 1f, false);
asm.SetPart(4, 82, -0.5f, 1.625f, false);
asm.SetPart(5, 107, 0.1875f, -1.25f, false);
asm.SetPart(6, 118, -0.4375f, -0.84375f, false);
break;
case 14:
asm.SetPart(0, 2, 0f, -0.78125f, false);
asm.SetPart(1, 28, -0.0625f, -1.625f, false);
asm.SetPart(2, 44, 0.0625f, -0.09375f, false);
asm.SetPart(3, 53, -0.5f, 1.0625f, false);
asm.SetPart(4, 82, -0.5f, 1.6875f, false);
asm.SetPart(5, 107, 0.0625f, -1.25f, false);
asm.SetPart(6, 117, -0.25f, -0.8125f, false);
break;
case 15:
asm.SetPart(0, 2, -0.25f, -0.65625f, false);
asm.SetPart(1, 28, 0.0625f, -1.625f, false);
asm.SetPart(2, 45, -0.0625f, 0f, false);
asm.SetPart(3, 53, -0.5f, 1.1875f, false);
asm.SetPart(4, 83, -0.5f, 1.875f, false);
asm.SetPart(5, 107, -0.0625f, -1.1875f, false);
asm.SetPart(6, 117, -0.0625f, -0.8125f, false);
break;
case 16:
asm.SetPart(0, 0, -0.4375f, -0.59375f, false);
asm.SetPart(1, 28, 0.1875f, -1.625f, false);
asm.SetPart(2, 45, -0.0625f, 0.0625f, false);
asm.SetPart(3, 53, -0.5f, 1.25f, false);
asm.SetPart(4, 83, -0.5f, 1.9375f, false);
asm.SetPart(5, 110, -0.375f, -1.1875f, false);
asm.SetPart(6, 117, 0.125f, -0.6875f, false);
break;
case 17:
asm.SetPart(0, 0, -0.625f, -0.59375f, false);
asm.SetPart(1, 28, 0.3125f, -1.625f, false);
asm.SetPart(2, 45, -0.0625f, 0f, false);
asm.SetPart(3, 53, -0.5f, 1.1875f, false);
asm.SetPart(4, 83, -0.5f, 1.875f, false);
asm.SetPart(5, 110, -0.5625f, -1.1875f, false);
asm.SetPart(6, 117, 0.3125f, -0.75f, false);
break;
case 18:
asm.SetPart(0, 0, -0.8125f, -0.59375f, false);
asm.SetPart(1, 28, 0.4375f, -1.625f, false);
asm.SetPart(2, 45, -0.0625f, -0.0625f, false);
asm.SetPart(3, 53, -0.5f, 1.125f, false);
asm.SetPart(4, 83, -0.5f, 1.8125f, false);
asm.SetPart(5, 110, -0.75f, -1.1875f, false);
asm.SetPart(6, 117, 0.5f, -0.8125f, false);
break;
case 19:
asm.SetPart(0, 0, -1f, -0.53125f, false);
asm.SetPart(1, 28, 0.5625f, -1.625f, false);
asm.SetPart(2, 46, -0.0625f, -0.03125f, false);
asm.SetPart(3, 53, -0.5625f, 1f, false);
asm.SetPart(4, 84, -0.4375f, 1.71875f, false);
asm.SetPart(5, 110, -0.9375f, -1.1875f, false);
asm.SetPart(6, 136, 0.75f, -0.71875f, false);
break;
case 20:
asm.SetPart(0, 1, -1.0625f, -0.53125f, false);
asm.SetPart(1, 17, 0.5625f, -1.4375f, false);
asm.SetPart(2, 46, -0.0625f, -0.09375f, false);
asm.SetPart(3, 53, -0.5625f, 0.9375f, false);
asm.SetPart(4, 84, -0.4375f, 1.65625f, false);
asm.SetPart(5, 110, -1.125f, -1.3125f, false);
asm.SetPart(6, 136, 0.75f, -0.71875f, false);
break;
case 21:
asm.SetPart(0, 1, -1.1875f, -0.53125f, false);
asm.SetPart(1, 17, 0.6875f, -1.4375f, false);
asm.SetPart(2, 46, -0.0625f, -0.15625f, false);
asm.SetPart(3, 53, -0.5625f, 0.875f, false);
asm.SetPart(4, 84, -0.4375f, 1.59375f, false);
asm.SetPart(5, 108, -1.1875f, -1.625f, false);
asm.SetPart(6, 136, 0.8125f, -0.65625f, false);
break;
case 22:
asm.SetPart(0, 1, -1.25f, -0.46875f, false);
asm.SetPart(1, 17, 0.8125f, -1.4375f, false);
asm.SetPart(2, 46, -0.0625f, -0.15625f, false);
asm.SetPart(3, 53, -0.5625f, 0.8125f, false);
asm.SetPart(4, 84, -0.4375f, 1.53125f, false);
asm.SetPart(5, 108, -1.0625f, -1.625f, false);
asm.SetPart(6, 136, 0.875f, -0.59375f, false);
break;
case 23:
asm.SetPart(0, 1, -1.3125f, -0.46875f, false);
asm.SetPart(1, 17, 0.9375f, -1.4375f, false);
asm.SetPart(2, 47, -0.125f, -0.21875f, false);
asm.SetPart(3, 53, -0.625f, 0.75f, false);
asm.SetPart(4, 85, -0.5625f, 1.4375f, false);
asm.SetPart(5, 108, -0.9375f, -1.625f, false);
asm.SetPart(6, 136, 0.9375f, -0.53125f, false);
break;
case 24:
asm.SetPart(0, 0, -1.25f, -0.53125f, false);
asm.SetPart(1, 25, 0.875f, -1.375f, false);
asm.SetPart(2, 47, -0.125f, -0.21875f, false);
asm.SetPart(3, 53, -0.625f, 0.75f, false);
asm.SetPart(4, 85, -0.5625f, 1.4375f, false);
asm.SetPart(5, 108, -0.8125f, -1.625f, false);
asm.SetPart(6, 136, 0.9375f, -0.59375f, false);
break;
case 25:
asm.SetPart(0, 0, -1.1875f, -0.53125f, false);
asm.SetPart(1, 25, 0.75f, -1.375f, false);
asm.SetPart(2, 47, -0.125f, -0.15625f, false);
asm.SetPart(3, 53, -0.625f, 0.8125f, false);
asm.SetPart(4, 85, -0.5625f, 1.5f, false);
asm.SetPart(5, 108, -0.6875f, -1.625f, false);
asm.SetPart(6, 136, 0.9375f, -0.53125f, false);
break;
case 26:
asm.SetPart(0, 0, -1.125f, -0.53125f, false);
asm.SetPart(1, 25, 0.6875f, -1.375f, false);
asm.SetPart(2, 47, -0.125f, -0.09375f, false);
asm.SetPart(3, 53, -0.625f, 0.875f, false);
asm.SetPart(4, 85, -0.5625f, 1.5625f, false);
asm.SetPart(5, 108, -0.5625f, -1.625f, false);
asm.SetPart(6, 136, 0.9375f, -0.53125f, false);
break;
case 27:
asm.SetPart(0, 0, -1f, -0.71875f, false);
asm.SetPart(1, 25, 0.5625f, -1.3125f, false);
asm.SetPart(2, 48, -0.125f, -0.15625f, false);
asm.SetPart(3, 53, -0.5625f, 0.9375f, false);
asm.SetPart(4, 82, -0.5625f, 1.5625f, false);
asm.SetPart(5, 108, -0.4375f, -1.625f, false);
asm.SetPart(6, 136, 0.875f, -0.71875f, false);
break;
case 28:
asm.SetPart(0, 0, -0.875f, -0.71875f, false);
asm.SetPart(1, 25, 0.4375f, -1.25f, false);
asm.SetPart(2, 48, -0.125f, -0.15625f, false);
asm.SetPart(3, 53, -0.5625f, 0.9375f, false);
asm.SetPart(4, 82, -0.5625f, 1.5625f, false);
asm.SetPart(5, 108, -0.3125f, -1.625f, false);
asm.SetPart(6, 136, 0.75f, -0.71875f, false);
break;
case 29:
asm.SetPart(0, 0, -0.6875f, -0.78125f, false);
asm.SetPart(1, 25, 0.3125f, -1.1875f, false);
asm.SetPart(2, 48, -0.125f, -0.09375f, false);
asm.SetPart(3, 53, -0.5625f, 1f, false);
asm.SetPart(4, 82, -0.5625f, 1.625f, false);
asm.SetPart(5, 108, -0.1875f, -1.625f, false);
asm.SetPart(6, 136, 0.5625f, -0.78125f, false);
break;
case 30:
asm.SetPart(0, 0, -0.5f, -0.78125f, false);
asm.SetPart(1, 25, 0.125f, -1.125f, false);
asm.SetPart(2, 48, -0.125f, -0.09375f, false);
asm.SetPart(3, 53, -0.5625f, 1f, false);
asm.SetPart(4, 82, -0.5625f, 1.625f, false);
asm.SetPart(5, 108, -0.0625f, -1.625f, false);
asm.SetPart(6, 117, 0.375f, -0.875f, false);
break;
case 31:
asm.SetPart(0, 0, -0.3125f, -0.90625f, false);
asm.SetPart(1, 25, -0.125f, -1f, false);
asm.SetPart(2, 49, -0.0625f, -0.09375f, false);
asm.SetPart(3, 53, -0.5f, 1.0625f, false);
asm.SetPart(4, 83, -0.5f, 1.75f, false);
asm.SetPart(5, 108, 0.0625f, -1.625f, false);
asm.SetPart(6, 117, 0.1875f, -1f, false);
break;
}
}
}