#include <emscripten/emscripten.h>

int GetParam();

int EMSCRIPTEN_KEEPALIVE  Test(int a)
{
  int param = GetParam();
  return a + 123 + param;
}