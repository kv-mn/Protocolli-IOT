namespace Protocolli.IoT.Drone.ApplicationCore.Models
{
    public class DroneCommand
    {
        public int DroneId { get; set; }
        public string Command { get; set; }
        public bool Value { get; set; }
        public long Timestamp { get; set; }

        static public DroneCommand RandomDroneCommand(int droneId)
        {
            //I comandi e i relativi valori sono:
            //    Apertura nuova corsa
            //        Command: "new-ride"
            //        Value: true
            //    Chiusura corsa corrente
            //        Command: "close-ride"
            //        Value: true
            //    Accensione / Spegnimento
            //        Command: "power"
            //        Value: true / false
            //    Rientro alla base
            //        Command: "return-to-base"
            //        Value: true
            //    Accensione / Spegnimento LED di posizione
            //        Command: "position-led"
            //        Value: true / false

            var random = new Random();
            var droneCommand = new DroneCommand();

            var commandArray = new List<string>() { "new-ride", "close-ride", "power", "return-to-base", "position-led" };
            var randomNumber = random.Next(commandArray.Count);

            droneCommand.DroneId = droneId;
            droneCommand.Command = commandArray[randomNumber];
            droneCommand.Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

            switch (randomNumber)
            {
                case 0:
                case 1:
                case 3:
                    droneCommand.Value = true;
                    break;

                case 2:
                case 4:
                    droneCommand.Value = random.Next(1) == 1 ? true : false;
                    break;

                default:
                    break;
            }

            return droneCommand;
        }
    }
}