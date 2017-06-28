using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

namespace RSPSample1
{
    class Program
    {

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

        private enum mir_sdr_If_kHzT
        {
            mir_sdr_IF_Zero = 0,
            mir_sdr_IF_0_450 = 450,
            mir_sdr_IF_1_620 = 1620,
            mir_sdr_IF_2_048 = 2048
        }

        //used with getDevices
        //perhaps change to a class?
        //private struct mir_sdr_DeviceT
        //{
        //   char SerNo;
        //   string DevNm;
        //   string hwVer;
        //   string devAvail;
       //}



        [DllImport("C:\\Program Files\\SDRplay\\API\\x86\\mir_sdr_api.dll")]
        private static extern mir_sdr_ErrT mir_sdr_SetParam(int ParamterId, int value);

        [DllImport("C:\\Program Files\\SDRplay\\API\\x86\\mir_sdr_api.dll")]
        private static extern mir_sdr_ErrT mir_sdr_Init(int gRdB, double fsMHz, double rfMHz, mir_sdr_Bw_MHzT bwType,
          mir_sdr_If_kHzT ifType, ref int samplesPerPacket);

        [DllImport("C:\\Program Files\\SDRplay\\API\\x86\\mir_sdr_api.dll")]
        private static extern mir_sdr_ErrT mir_sdr_SetDcMode(int dcCal, int speedUp);

        [DllImport("C:\\Program Files\\SDRplay\\API\\x86\\mir_sdr_api.dll")]
        private static extern mir_sdr_ErrT mir_sdr_SetDcTrackTime(int trackTime);

        [DllImport("C:\\Program Files\\SDRplay\\API\\x86\\mir_sdr_api.dll")]
        private static extern mir_sdr_ErrT mir_sdr_ReadPacket(short[] xi, short[] xq, ref uint firstSampleNum,
            ref int grChanged, ref int rfChanged, ref int fsChanged);

        [DllImport("C:\\Program Files\\SDRplay\\API\\x86\\mir_sdr_api.dll")]
        private static extern mir_sdr_ErrT mir_sdr_Uninit();


        //not working yet
        //[DllImport("C:\\Program Files\\SDRplay\\API\\x86\\mir_sdr_api.dll")]
        //private static extern mir_sdr_ErrT mir_sdr_GetDevices(mir_sdr_DeviceT *devices, ref uint numDevs, ref uint maxDevs);


        //[DllImport("C:\\Program Files\\SDRplay\\API\\x86\\mir_sdr_api.dll")]
        //private static extern mir_sdr_ErrT mir_sdr_SetDeviceIdx(uint idx);

        //[DllImport("C:\\Program Files\\SDRplay\\API\\x86\\mir_sdr_api.dll")]
        //private static extern mir_sdr_ErrT mir_sdr_ReleaseDeviceIdx();

        static void Main(string[] args)
        {
            const int DEFAULT_SAMPLE_RATE = 2048000;
            const int DEFAULT_BUF_LENGTH = (336 * 2);


            //make loop (or if/else statements if we have to) here: for (int z=0; z<8;z++)
            //{
            //mir_sdr_GetDevices()    //must be called before usage of set, returns idxs
            //idx=z;
            //mir_sdr_SetDeviceIdx(idx);
            //
            // all of this code below
            //if z=0: output filename1, if z=1 filename2, ect.
            // 
            // mir_sdr_ReleaseDeviceIdx() //important!!!
            //}


            //actual code attempt:

            //for (int z = 0; z < 8; z++)
            //{
            //unsure how to do this with pointers. (array of device objects ourDevices)
            //uint numberDevs=1;
            //uint maximumDevs=8;
            //getDevices = mir_sdr_GetDevices(ref ourDevices, ref numberDevs, ref maximumDevs);
            //if (getDevices != mir_sdr_ErrT.mir_sdr_Success)
            //{
            //  Console.WriteLine("Failed to get the IDs of the devices.");
            //  return;
            //}
            //
            //uint myIdx=z;
            //myDevice = mir_srd_SetDeviceIdx(ref myIdx)
            //if (myDevice != mir_sdr_ErrT.mir_sdr_Success)
            //{
            //  Console.WriteLine("Failed to set Device ID.");
            //  return;
            //}


            bool do_exit = false;
            //i changed this from 0, otherwise it just reads forever.
            //if you have this it reads 500 Mbs and then exits. (can change to however many bytes we want).
            uint bytes_to_read = 500000;

            // I and Q values:
            short[] ibuf;
            short[] qbuf;

            uint firstSample = 0;
            int samplesPerPacket = 0, grChanged = 0, fsChanged = 0, rfChanged = 0;

            //change output file depending on the device.
            //if (z=0)
            //{ string filename = "filename1.raw"; }
            // else if (z=1)
            //{ string filename = "filename2.raw"; }
            // else if (z=2)
            //{ string filename = "filename3.raw"; }
            // else if (z=3)
            //{ string filename = "filename3.raw"; }
            // else if (z=4)
            //{ string filename = "filename4.raw"; }
            // else if (z=5)
            //{ string filename = "filename5.raw"; }
            // else if (z=6)
            //{ string filename = "filename6.raw"; }
            // else if (z=7)
            //{ string filename = "filename7.raw"; }
            // else if (z=8)
            //{ string filename = "filename8.raw"; }
            // else
            //{Console.WriteLine("For loop is messed up."); return;}


            string filename = "filename1.raw"; // output file, containing raw IQ samples
            int n_read;
            mir_sdr_ErrT r;

            int gain = 50;
            FileStream file = new FileStream(filename, FileMode.Create);
            BinaryWriter binWriter = new BinaryWriter(file);

            byte[] buffer = new byte[DEFAULT_BUF_LENGTH];
            uint frequency = 104300000; // frequency: 104.3 MHZ (a local FM station)
            uint samp_rate = DEFAULT_SAMPLE_RATE;

            int i, j;

            r = mir_sdr_Init(40, 2.0, 100.00, mir_sdr_Bw_MHzT.mir_sdr_BW_1_536, mir_sdr_If_kHzT.mir_sdr_IF_Zero,
                        ref samplesPerPacket);

            if (r != mir_sdr_ErrT.mir_sdr_Success)
            {
                Console.WriteLine("Failed to open SDRplay RSP device.");

                return;
            }
            mir_sdr_Uninit();

            mir_sdr_SetParam(201, 1);
            mir_sdr_SetParam(202, 0);
            r = mir_sdr_Init(gain, (samp_rate / 1e6), (frequency / 1e6),
                           mir_sdr_Bw_MHzT.mir_sdr_BW_1_536, mir_sdr_If_kHzT.mir_sdr_IF_Zero, ref samplesPerPacket);

            if (r != mir_sdr_ErrT.mir_sdr_Success)
            {
                Console.WriteLine("Failed to open SDRplay RSP device.");

                return;
            }
            mir_sdr_SetDcMode(4, 0);
            mir_sdr_SetDcTrackTime(63);
            ibuf = new short[samplesPerPacket];
            qbuf = new short[samplesPerPacket];

            Console.WriteLine("Writing samples...");
            while (!do_exit)
            {
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

                //Console.WriteLine(bytes_to_read);
                //Console.WriteLine((uint)n_read);

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

            //release Device
            //deviceRelease = mir_srd_ReleaseDeviceIdx();
            //if (deviceRelease != mir_sdr_ErrT.mir_sdr_Success)
            //{
            //  Console.WriteLine("Failed to release (or access for that matter) the Device.");
            //  return;
            //}


            mir_sdr_Uninit();
            //}
        }
    }
}
