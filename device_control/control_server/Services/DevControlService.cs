using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevControl;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace control_server
{
    public class DevControlService : DevControl.DevControlService.DevControlServiceBase
    {
        private readonly ILogger<DevControlService> _logger;
        public DevControlService(ILogger<DevControlService> logger)
        {
            _logger = logger;
        }

        public override Task<ResponseGetI2C> GetI2C(RequestGetI2C request, ServerCallContext context)
        {
            try
            {
                var b = common.I2CController.Read(
                            request.BusId, request.DevAddress, request.RegisterAddress, request.ReadLength);
                return Task.FromResult(new ResponseGetI2C()
                {
                    Query = request,
                    Response = Google.Protobuf.ByteString.CopyFrom(b)
                });
            }
            catch (Exception e)
            {
                return Task.FromException<ResponseGetI2C>(e);
            }
        }

        public override Task<MGPIO> GetGPIO(MGPIO request, ServerCallContext context)
        {
            try
            {
                var s = common.GPIOController.ReadGPIO(request.IoPin);
                return Task.FromResult(new MGPIO()
                {
                    IoPin = request.IoPin,
                    State = s
                });
            }
            catch (Exception e)
            {
                return Task.FromException<MGPIO>(e);
            }
        }

        public override Task<MEmpty> SetGPIO(MGPIO request, ServerCallContext context)
        {
            try
            {
                common.GPIOController.SetGPIO(request.IoPin, request.State);
                return Task.FromResult(new MEmpty());
            }
            catch (Exception e)
            {
                return Task.FromException<MEmpty>(e);
            }
        }
    }
}
