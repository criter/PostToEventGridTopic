using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubmitToEventGridTopic
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //  Can be passed on the command line as well    FIRST parameter is endpoint SECOND is saskey THIRD is repeat count
                string gridTopicEndpoint = @"https://<grid topic url>.eventgrid.azure.net/api/events";
                string sasKey = @"<your sas key>";

                int repeatTimes = 1;
                if (args.Length > 2)
                {
                    int.TryParse(args[2], out repeatTimes);
                }
                Console.WriteLine($"Will repeat send event {repeatTimes}");

                if (args.Length > 0)
                {
                    gridTopicEndpoint = args[0];
                }
                if (args.Length > 1)
                {
                    sasKey = args[1];
                }
                Console.WriteLine($"Grid Topic Endpoint: {gridTopicEndpoint}");
                Console.WriteLine($"sasKey: {sasKey}");


                PostToEventGrid(repeatTimes, gridTopicEndpoint, sasKey);

                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine($" StackTrace: {ex.StackTrace}");

                Console.WriteLine($" call app from command line with 3 parameters");
                Console.WriteLine($"   first URL of the Grid Topic Endpoint");
                Console.WriteLine($"   second sasKey for the Grid Topic");
                Console.WriteLine($"   third times to repeat the call");
            }
        }


        static void PostToEventGrid(int loopCount, string gridTopicEndpoint, string sasKey) { 

            string subject = @"serverless/training/lab3/order";
            string eventType = @"orderSubmit";

            for (int i = 0; i < loopCount; i++)
            {

                new EventGridObject(gridTopicEndpoint, sasKey, subject, eventType)
                {
                    Data = new
                    {
                        orderid = @"value1",
                        itemid = 123,
                        itemdesc = "Fluffy Bunny",
                        quantity = 3,
                        itemprice = 9.99,
                        extendedprice = 29.97,
                        customerid = Guid.NewGuid(),
                    }
                }.SendAsync()
                /* wait for it to complete so it is synchronous*/ .Wait(-1);
                Console.WriteLine($"Sent order event {i+1}");
            }

        }
    }
}


