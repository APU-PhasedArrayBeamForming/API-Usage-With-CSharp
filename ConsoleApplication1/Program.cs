﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using static System.Object;
using static System.Delegate;

namespace RSPSample1
{
    class Program
    {
        //used with streaminit (last param)
        private static unsafe void* cbContext2;

        //used with all API usages: is returned to say whether API worked or not.
        enum mir_sdr_ErrT
        {
            mir_sdr_Success = 0,
            mir_sdr_Fail = 1,
            mir_sdr_InvalidParam = 2,
            mir_sdr_OutOfRange = 3,
            mir_sdr_GainUpdateError = 4,
            mir_sdr_RfUpdateError = 5,
            mir_sdr_FsUpdateError = 6,
            mir_sdr_HwError = 7,
            mir_sdr_AliasingError = 8,
            mir_sdr_AlreadyInitialised = 9,
            mir_sdr_NotInitialised = 10
        }

 //used to specify bandwidth
        private enum mir_sdr_Bw_MHzT
        {
            mir_sdr_BW_0_200 = 200,
            mir_sdr_BW_0_300 = 300,
            mir_sdr_BW_0_600 = 600,
            mir_sdr_BW_1_536 = 1536,
            mir_sdr_BW_5_000 = 5000,
            mir_sdr_BW_6_000 = 6000,
            mir_sdr_BW_7_000 = 7000,
            mir_sdr_BW_8_000 = 8000
        }

 //specifies the IF to be used (IF means Intermediate Frequency, we use 0)
        private enum mir_sdr_If_kHzT
        {
            mir_sdr_IF_Zero = 0,
            mir_sdr_IF_0_450 = 450,
            mir_sdr_IF_1_620 = 1620,
            mir_sdr_IF_2_048 = 2048
        }

 //all of the above are used with getting data from device. (came with C# program initiallly.) below is from us.
 
 //used with getDevices
        public unsafe struct mir_sdr_DeviceT
        {
            public char* SerNo;
            public char* DevNm;
            public byte hwVer;
            public byte devAvail;

            public mir_sdr_DeviceT(char* p1, char* p2, byte p3, byte p4)
            {
                SerNo = p1;
                DevNm = p2;
                hwVer = p3;
                devAvail = p4;
            }
        }

 //used to set band type (with what function idk) (not implemented yet)
        private enum mir_sdr_RSPII_BandT
        {
            mir_sdr_RSPII_BAND_UNKNOWN = 0,
            mir_sdr_RSPII_BAND_AM_LO = 1,
            mir_sdr_RSPII_BAND_AM_MID = 2,
            mir_sdr_RSPII_BAND_AM_HI = 3,
            mir_sdr_RSPII_BAND_VHF = 4,
            mir_sdr_RSPII_BAND_3 = 5,
            mir_sdr_RSPII_BAND_X_LO = 6,
            mir_sdr_RSPII_BAND_X_MID = 7,
            mir_sdr_RSPII_BAND_X_HI = 8,
            mir_sdr_RSPII_BAND_4_5 = 9,
            mir_sdr_RSPII_BAND_L = 10
        }

   //used with streaminit
         private enum mir_sdr_SetGrModeT
        {
            mir_sdr_USE_SET_GR = 0,
            mir_sdr_USE_SET_GR_ALT_MODE = 1,
            mir_sdr_USE_RSP_SET_GR = 2,
        }
        
//Importing each function from the API.
 //mir_sdr_SetParam 
        [DllImport("C:\\Program Files\\SDRplay\\API\\x86\\mir_sdr_api.dll")]
        private static extern mir_sdr_ErrT mir_sdr_SetParam(int ParamterId, int value);
 
 //mir_sdr_Init
        [DllImport("C:\\Program Files\\SDRplay\\API\\x86\\mir_sdr_api.dll")]
        private static extern mir_sdr_ErrT mir_sdr_Init(int gRdB, double fsMHz, double rfMHz, mir_sdr_Bw_MHzT bwType,
          mir_sdr_If_kHzT ifType, ref int samplesPerPacket);

 //mir_sdr_SetDcMode
        [DllImport("C:\\Program Files\\SDRplay\\API\\x86\\mir_sdr_api.dll")]
        private static extern mir_sdr_ErrT mir_sdr_SetDcMode(int dcCal, int speedUp);

 //mir_sdr_SetDcTrackTime
        [DllImport("C:\\Program Files\\SDRplay\\API\\x86\\mir_sdr_api.dll")]
        private static extern mir_sdr_ErrT mir_sdr_SetDcTrackTime(int trackTime);

 //mir_sdr_ReadPacket
        [DllImport("C:\\Program Files\\SDRplay\\API\\x86\\mir_sdr_api.dll")]
        private static extern mir_sdr_ErrT mir_sdr_ReadPacket(short[] xi, short[] xq, ref uint firstSampleNum,
            ref int grChanged, ref int rfChanged, ref int fsChanged);

 //mir_sdr_Uninit
        [DllImport("C:\\Program Files\\SDRplay\\API\\x86\\mir_sdr_api.dll")]
        private static extern mir_sdr_ErrT mir_sdr_Uninit();


//multiple devices (our implementations)
 //mir_sdr_GetDevices
        [DllImport("C:\\Program Files\\SDRplay\\API\\x86\\mir_sdr_api.dll")]
        private static extern mir_sdr_ErrT mir_sdr_GetDevices(mir_sdr_DeviceT[] devices, ref uint numDevs, uint maxDevs);

 //mir_sdr_SetDeviceIdx
        [DllImport("C:\\Program Files\\SDRplay\\API\\x86\\mir_sdr_api.dll")]
        private static extern mir_sdr_ErrT mir_sdr_SetDeviceIdx(uint idx);

 //mir_sdr_ReleaseDeviceIdx
        [DllImport("C:\\Program Files\\SDRplay\\API\\x86\\mir_sdr_api.dll")]
        private static extern mir_sdr_ErrT mir_sdr_ReleaseDeviceIdx();

        //make AM?
        //[DllImport("C:\\Program Files\\SDRplay\\API\\x86\\mir_sdr_api.dll")]
        //function here:idk which one.
//functions used with streaminit
 //mir_sdr_DecimateControl
        [DllImport("C:\\Program Files\\SDRplay\\API\\x86\\mir_sdr_api.dll")]
        private static extern mir_sdr_ErrT mir_sdr_DecimateControl(uint enable, uint decimationFactor, uint wideBandSignal);

 //mir_sdr_StreamCallback_T
        [DllImport("C:\\Program Files\\SDRplay\\API\\x86\\mir_sdr_api.dll")]
        private unsafe static extern void mir_sdr_StreamCallback_t(short *xi, short *xq, uint firstSampleNum,
        int grChanged, int rfChanged, int fsChanged,
        uint numSamples, uint reset,
        void *cbContext);
//mir_sdr_streamDelegate (function pointer)
   
        unsafe delegate void mir_sdr_StreamCallbackDel_t(short* xi, short* xq, uint firstSampleNum,
        int grChanged, int rfChanged, int fsChanged,
        uint numSamples, uint reset,
        void* cbContext);
 
 //mir_sdr_GainChangeCallback_T
        [DllImport("C:\\Program Files\\SDRplay\\API\\x86\\mir_sdr_api.dll")]
        private unsafe static extern void mir_sdr_GainChangeCallback_t (uint gRidx, uint gRdB,
        uint lnaGRdB, void *cbContext);

 //mir_sdr_GainDelegate (function pointer)
        unsafe delegate void mir_sdr_GainChangeDel_t (uint gRidx, uint gRdB,
        uint lnaGRdB, void *cbContext);

 //mir_sdr_StreamInit
    //doesn't work yet..problem with last parameter (void pointer...also in gaincallback and streamcallback.
        [DllImport("C:\\Program Files\\SDRplay\\API\\x86\\mir_sdr_api.dll")]
        private unsafe static extern mir_sdr_ErrT mir_sdr_StreamInit(ref int gRdB, double fsMHz, double rfMHz, mir_sdr_Bw_MHzT bwType,
            mir_sdr_If_kHzT ifType, int LNAstate, ref int gRdBsystem,
            mir_sdr_SetGrModeT setGrMode, ref int samplesPerPacket,
           mir_sdr_StreamCallbackDel_t StreamCbFn,
            mir_sdr_GainChangeDel_t GainChangeCbFn, void* cbContext);

 //mir_sdr_StreamUninit
        [DllImport("C:\\Program Files\\SDRplay\\API\\x86\\mir_sdr_api.dll")]
        private static extern mir_sdr_ErrT mir_sdr_StreamUninit();

 //mir_sdr_setGr (within streaminit there is three
           [DllImport("C:\\Program Files\\SDRplay\\API\\x86\\mir_sdr_api.dll")] 
            private static extern mir_sdr_ErrT mir_sdr_RSP_SetGr(int gRdB, int LNAstate, int abs, int syncUpdate);


        static unsafe void Main(string[] args)
        {
            const int DEFAULT_SAMPLE_RATE = 2048000;                                    
            const int DEFAULT_BUF_LENGTH = (336 * 2);                                   //default buffer length

            //for loop begins
            int numberOfSDRs = 2;                                                       //only change this line to add more SDRs
            for (int z = 0; z < numberOfSDRs; z++)                                      //for # of SDRs, read and output a file
            {
            mir_sdr_ErrT r;                                                             //ErrT object initialized (r is used to check program API success)
            
            mir_sdr_DeviceT[] ourDevices;                                               //array of Device structs (look at mir_sdr_DeviceT above)
            ourDevices = new mir_sdr_DeviceT[numberOfSDRs];                             //expect correct # of devices from what we told it.

            uint numberDevs=1;                                                          //is later changed to # of devices found by API when function is called.
            uint maximumDevs=8;                                                         //maximum number of devices we want.
            r = mir_sdr_GetDevices( ourDevices, ref numberDevs, maximumDevs);           //fcn: takes in array of device structs, a var to store # of devices found,
            if (r != mir_sdr_ErrT.mir_sdr_Success)                                      //and maximum devices we want. If it worked, r= success.
            {
              Console.WriteLine("Failed to get the IDs of the devices.");               //otherwise, print our failure.
              
            }
            
            uint myIdx=Convert.ToUInt32(z);                                             //convert for loop counter (z) to uint, store in myIdx
                r = mir_sdr_SetDeviceIdx(myIdx);                                        //fcn: takes in which device you want (myIdx), -we use # of devices in for loop
            if (r != mir_sdr_ErrT.mir_sdr_Success)                                      //returns success or not.
            {
              Console.WriteLine("Failed to set Device ID.");
              
            }
            //proves it is using different devices                                      //we trust that select device is working, so we trust it is using different
            //Console.WriteLine(myIdx);                                                 //devices if this outputs different numbers(if you uncomment it outputs 0 first
                                                                                        //time and 1 second time and so on) and r returns success.

            //to check if it uses two different devices, doesnt work.                   //i wanted to get the device Serial Numbers to further prove using dif devices.
            //Console.WriteLine(*ourDevices[z].SerNo);                                  //however, pointers are a pain and so SerNo and DevNm don't work here while
            //Console.WriteLine(numberDevs); devices found with getdevices              //other DeviceT struct values are usable, we just trust SetDeviceIdk above is working.

            bool do_exit = false;                                                       //when we want to exit, we set this true.
            //i changed this from 0, otherwise it just reads forever.
            //if you have this it reads 500 Mbs and then exits. (can change to however many bytes we want).
            uint bytes_to_read = 500000;

            // I and Q values:
            short[] ibuf;
            short[] qbuf;

            uint firstSample = 0;
            int samplesPerPacket = 0, grChanged = 0, fsChanged = 0, rfChanged = 0;
            string filename= @"C:\\Users\\Justin\\Documents\\Visual Studio 2015\\Projects\\ConsoleApplication1\\ConsoleApplication1\\bin\\Debug\\filename1.raw";
            //change output file depending on the device.
            if (z==0)
            { filename = @"C:\\Users\\Justin\\Documents\\Visual Studio 2015\\Projects\\ConsoleApplication1\\ConsoleApplication1\\bin\\Debug\\filename1.raw"; }
             else if (z==1)
            { filename = @"C:\\Users\\Justin\\Documents\\Visual Studio 2015\\Projects\\ConsoleApplication1\\ConsoleApplication1\\bin\\Debug\\filename2.raw"; }
             else if (z==2)
            { filename = @"C:\\Users\\Justin\\Documents\\Visual Studio 2015\\Projects\\ConsoleApplication1\\ConsoleApplication1\\bin\\Debug\\filename3.raw"; }
             else if (z==3)
            { filename = @"C:\\Users\\Justin\\Documents\\Visual Studio 2015\\Projects\\ConsoleApplication1\\ConsoleApplication1\\bin\\Debug\\filename4.raw"; }
             else if (z==4)
            { filename = @"C:\\Users\\Justin\\Documents\\Visual Studio 2015\\Projects\\ConsoleApplication1\\ConsoleApplication1\\bin\\Debug\\filename5.raw"; }
             else if (z==5)
            { filename = @"C:\\Users\\Justin\\Documents\\Visual Studio 2015\\Projects\\ConsoleApplication1\\ConsoleApplication1\\bin\\Debug\\filename6.raw"; }
             else if (z==6)
            { filename = @"C:\\Users\\Justin\\Documents\\Visual Studio 2015\\Projects\\ConsoleApplication1\\ConsoleApplication1\\bin\\Debug\\filename7.raw"; }
             else if (z==7)
            { filename = @"C:\\Users\\Justin\\Documents\\Visual Studio 2015\\Projects\\ConsoleApplication1\\ConsoleApplication1\\bin\\Debug\\filename8.raw"; }
             else
            {Console.WriteLine("For loop is messed up."); }

            
            int n_read;                                                                 //# of read bytes
            

            int gain = 50;                                                              //gain reduction related idk
            FileStream file = new FileStream(filename, FileMode.Create);                //what writes to file.
            BinaryWriter binWriter = new BinaryWriter(file);                            //what writes to buffer (in binary)

            byte[] buffer = new byte[DEFAULT_BUF_LENGTH];                               //buffer creation
            uint frequency = 104300000;                                                 // frequency: 104.3 MHZ (a local FM station)
            uint samp_rate = DEFAULT_SAMPLE_RATE;
                                                                                        //check to see if we connect to SDR(s)
                                                                                        //mir_SDR_init: fcn: takes in gain reduction, sample frequency (MHz),
            int i, j;                                                                   //tuner frequency(MHz), bandwidth, ifType (see top),
                                                                                        //and the number of samples to be returned for each readpacket

            //attempting to replace mir_sdr_init with mir_sdr_streaminit
            int gainReduction=50;
            double fsMegaHz=0;
            double rfMegaHz=0;
            int LNAposition=4;
            int gainReductionSys=0;
            mir_sdr_StreamCallbackDel_t StreamCbFn2= new mir_sdr_StreamCallbackDel_t(mir_sdr_StreamCallback_t);
            mir_sdr_GainChangeDel_t GainChangeCbFn2= new mir_sdr_GainChangeDel_t(mir_sdr_GainChangeCallback_t);

            r=mir_sdr_StreamInit(ref gainReduction, fsMegaHz, rfMegaHz, mir_sdr_Bw_MHzT.mir_sdr_BW_1_536,
                mir_sdr_If_kHzT.mir_sdr_IF_Zero, LNAposition, ref gainReductionSys,
                mir_sdr_SetGrModeT.mir_sdr_USE_SET_GR, ref samplesPerPacket,
                StreamCbFn2,
                GainChangeCbFn2, cbContext2);
            
             if (r != mir_sdr_ErrT.mir_sdr_Success)                                      //do we connect?
            {
                Console.WriteLine("Failed to open SDRplay RSP device with streaminit.");

            }
                else
                {
                    Console.WriteLine("Success!");
                }
            mir_sdr_StreamUninit();                                  

           //end streaminit attempt
                     
            r = mir_sdr_Init(40, 2.0, 100.00, mir_sdr_Bw_MHzT.mir_sdr_BW_1_536, mir_sdr_If_kHzT.mir_sdr_IF_Zero,
                        ref samplesPerPacket);

            if (r != mir_sdr_ErrT.mir_sdr_Success)                                      //do we connect?
            {
                Console.WriteLine("Failed to open SDRplay RSP device.");

            }
            mir_sdr_Uninit();                                                           //end connection check.

            mir_sdr_SetParam(201, 1);                                                   //fcn: set different settings on SDR before init. look online.
            mir_sdr_SetParam(202, 0);                                                   //takes in paramID and value to set.
            r = mir_sdr_Init(gain, (samp_rate / 1e6), (frequency / 1e6),                //init again for real.
                           mir_sdr_Bw_MHzT.mir_sdr_BW_1_536, mir_sdr_If_kHzT.mir_sdr_IF_Zero, ref samplesPerPacket);
            if (r != mir_sdr_ErrT.mir_sdr_Success)
            {
                Console.WriteLine("Failed to open SDRplay RSP device.");                //it work?

            }
                                                                                        //fcn: reduce sample_rate by factor
            //decimate                                                                  
            uint enabledecimation = 0;                                                  //0 or 1 (on)
            uint factorofdecimation = 16;                                                //factor of: 2,4,8,16,32
            uint bandwidesignal = 0;                                                    //half band filter or averaging (1 or 0)
            r = mir_sdr_DecimateControl(enabledecimation, factorofdecimation, bandwidesignal);

            if (r != mir_sdr_ErrT.mir_sdr_Success)
            {
                Console.WriteLine("failed to decimate.");

            }

                // configure DC tracking in tuner
                mir_sdr_SetDcMode(4, 0);                                                // select one-shot tuner DC offset correction with speedup
                mir_sdr_SetDcTrackTime(63);                                             // with maximum tracking time
                ibuf = new short[samplesPerPacket];                                     //match each buffer with size.
            qbuf = new short[samplesPerPacket];

            Console.WriteLine("Writing samples...");                                    //their while loop which writes 8 bit I/Q to file.
            while (!do_exit)
            {                                                                           //get packet data from API
                r = mir_sdr_ReadPacket(ibuf, qbuf, ref firstSample, ref grChanged, ref rfChanged,
                                    ref fsChanged);

                if (r != mir_sdr_ErrT.mir_sdr_Success)
                {
                    Console.WriteLine("WARNING: ReadPacket failed.");
                    break;
                }

                j = 0;
                for (i = 0; i < samplesPerPacket; i++)
                {

                    // I and Q values are 16-bits values; here we convert them to 8-bits to be compatible with RTL-SDR format.
                    buffer[j++] = (byte)(ibuf[i] >> 8);
                    buffer[j++] = (byte)(qbuf[i] >> 8);
                }

                n_read = (samplesPerPacket * 2);

                if ((bytes_to_read > 0) && (bytes_to_read <= (uint)n_read))
                {
                    n_read = (int)bytes_to_read;

                    do_exit = true;
                }

                binWriter.Write(buffer);

                if (bytes_to_read > 0)
                    bytes_to_read -= (uint)n_read;
            }

            file.Flush();
            file.Close();
            binWriter.Close();                                                  //this is to reset variables to prepare for next for loop iteration.
            binWriter.Dispose();

            //release Device
            r = mir_sdr_ReleaseDeviceIdx();                                     //let go of device so we can pick a different one. (IMPORTANT)
            if (r != mir_sdr_ErrT.mir_sdr_Success)
            {
              Console.WriteLine("Failed to release (or access for that matter) the Device.");
             
            }
            //clear arrays so they can be used again in next iteration of for loop.
            Array.Clear(ibuf,0,ibuf.Length);
            Array.Clear(qbuf,0,qbuf.Length);
            Array.Clear(buffer,0,buffer.Length);

            mir_sdr_Uninit();
            }
            //read console (need to press button in console if uncommented.)
            Console.Read();
        }
    }
}
