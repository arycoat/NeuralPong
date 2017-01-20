using System;
using System.Threading;
using System.Windows.Forms;

namespace NeuralPong
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            PongTrainer trainer = new PongTrainer();

            // loop here forever
            //for (int coin = 100; coin > 0; )
            //{
            //    trainer.timer_tick(null);
            //    if (trainer.left_hits_per_round > 100)
            //    {
            //        trainer.left_hits_per_round = 0;
            //        trainer.start();
            //        coin--;
            //    }
            //    // add a sleep for 100 mSec to reduce CPU usage
            //    Thread.Sleep(1);
            //}
            //
            //Console.ReadLine();

            //Game window = new Game(640, 480, trainer);
            //window.Run(60);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainApp(trainer));
        }
    }
}
