using System.Collections;

namespace ProducerSocket
{
    class Program
    {
    static consume cr;
    
     static void Main(string[] args){
       Bqueue bqueue=new Bqueue(10);
        Produce pr=new Produce();

          cr=new consume();
          Random random=new Random(1);

        Thread t=new Thread(callConsumer);
        t.Start(random.Next(1000));
        for(int i=0;i<100;i++){
            //Console.WriteLine($"Producing   {i}");
           // Thread.Sleep(random.Next(1000));
            pr.Producer(i);
        }
        static  void callConsumer(object tim)
        {
           
            while(true){
           // Thread.Sleep((int)tim);
            object ob=cr.consumer();
            //Console.WriteLine($"\t\t\t\t\tConsuming {ob}");
            
            }
        }

     }



}
     class Produce{
        
        public void Producer(object o){
          
            
           
           lock(Bqueue.listLock){
            while(Bqueue.queue.Count>10){
                
                Console.WriteLine("Buffer is full plese wait while we consume it");
               Monitor.Wait(Bqueue.listLock);
            }
            // if(Bqueue.queue.Count<=10){
                Console.WriteLine($"Enqueuing    {o}");
                 Bqueue.queue.Enqueue(o);
                 Monitor.Pulse(Bqueue.listLock);
           // }else{
               // Console.WriteLine("Buffer is full plese wait while we consume it");
            //    Monitor.Pulse(Bqueue.listLock);
              //   Monitor.Wait(Bqueue.listLock);
          //  }
           
           }
        }
     }



     class consume{
            public object consumer(){
                 
                    lock(Bqueue.listLock){
                            while(Bqueue.queue.Count==0){
                                Monitor.Wait(Bqueue.listLock);
                            }
                            object t= Bqueue.queue.Dequeue();
                            Console.WriteLine($"\t\t\t\t\tConsuming {t}");            
                            Monitor.Pulse(Bqueue.listLock);
                            return t;
                    }
            }
     }



     class Bqueue{
            public static readonly object listLock=new object();
           public static Queue queue;
           public Bqueue(int size){
            queue=new Queue(size);
           }
           
     
 }
}