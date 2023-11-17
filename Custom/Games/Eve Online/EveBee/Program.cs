using EveBee.Scenarios;

Thread.Sleep(10000);

while (true)
{
    ScenariosManager.Tick();
    Thread.Sleep(1000);
}