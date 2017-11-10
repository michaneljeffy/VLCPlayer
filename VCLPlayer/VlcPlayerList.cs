using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace VCLPlayer
{
    class VlcPlayerList
    {
        private IntPtr libvlc_instance_;
        private IntPtr libvlc_media_list_player_ = IntPtr.Zero;
        private IntPtr libvlc_media_player_;
        public VlcPlayerList(string pluginPath)
        {
            string plugin_arg = "--plugin-path=" + pluginPath;
            string[] arguments = { "-I", "dummy", "--ignore-config", "--no-video-title", plugin_arg };
            libvlc_instance_ = LibVlcAPI.libvlc_new(arguments);
            libvlc_media_player_ = LibVlcAPI.libvlc_media_player_new(libvlc_instance_);
            libvlc_media_list_player_ = VlcPlayerListAPI.libvlc_media_list_player_new (libvlc_instance_);
        }

        public void SetRenderWindow(int wndHandle)
        {
            try
            {
                if (libvlc_instance_ != IntPtr.Zero && wndHandle != 0)
                {
                    VlcPlayerListAPI.libvlc_media_list_player_set_media_player(libvlc_media_list_player_, libvlc_media_player_);
                    VlcPlayerListAPI.libvlc_media_player_set_hwnd(libvlc_media_player_, wndHandle);
                  
                }
            }

            catch(Exception ex)
            {
                Console.WriteLine(ex.Message );
            }
           
        }

        public void PlayFiles(string [] files, int wndHandle)
        {
            try
            {

                //IntPtr  libvlc_media_list = new List<IntPtr>();
                IntPtr list = VlcPlayerListAPI.libvlc_media_list_new(libvlc_instance_);


                for (int i=0; i< files.Length ; i++)
                {
                   
                    IntPtr libvlc_media = VCLPlayer.LibVlcAPI.libvlc_media_new_path(libvlc_instance_, files[i]);
                    //  LibVlcAPI.libvlc_media_player_set_media(libvlc_media_player_, libvlc_media);
                    VlcPlayerListAPI.libvlc_media_list_add_media(list,libvlc_media);
                    // LibVlcAPI.libvlc_media_release(libvlc_media);    
                    //VlcPlayerListAPI.libvlc_media_player_set_hwnd(libvlc_media, wndHandle);
                }
           
                VlcPlayerListAPI.libvlc_media_list_player_set_media_list(libvlc_media_list_player_, list);

              
                VlcPlayerListAPI.libvlc_media_list_player_set_playback_mode(libvlc_media_list_player_,1);
               
               
                VlcPlayerListAPI.libvlc_media_list_player_play(libvlc_media_list_player_);

               // IntPtr media_player = VlcPlayerListAPI.libvlc_media_list_player_get_media_player(libvlc_media_list_player_);
                //LibVlcAPI.libvlc_media_player_play(libvlc_media_player_);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
         
        }

        public void PlayNext(int index)
        {
            VlcPlayerListAPI.libvlc_media_list_player_next(libvlc_media_list_player_);
        }
    }

    internal static  class VlcPlayerListAPI
    {
        internal struct PointerToArrayOfPointerHelper
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
            public IntPtr[] pointers;
        }

        
        internal enum libvlc_playback_mode_t
        {
            libvlc_playback_mode_default,
            libvlc_playback_mode_loop,
            libvlc_playback_mode_repeat
        }
        public static IntPtr libvlc_new(string[] arguments)
        {
            PointerToArrayOfPointerHelper argv = new PointerToArrayOfPointerHelper();
            argv.pointers = new IntPtr[11];
            for (int i = 0; i < arguments.Length; i++)
            {
                argv.pointers[i] = Marshal.StringToHGlobalAnsi(arguments[i]);
            }
            IntPtr argvPtr = IntPtr.Zero;
            try
            {
                int size = Marshal.SizeOf(typeof(PointerToArrayOfPointerHelper));
                argvPtr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(argv, argvPtr, false);
                return libvlc_new(arguments.Length, argvPtr);
            }
            finally
            {
                for (int i = 0; i < arguments.Length + 1; i++)
                {
                    if (argv.pointers[i] != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(argv.pointers[i]);
                    }
                }
                if (argvPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(argvPtr);
                }
            }
        }
        public static IntPtr libvlc_media_new_path(IntPtr libvlc_instance, string path)
        {
            IntPtr pMrl = IntPtr.Zero;
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(path);
                pMrl = Marshal.AllocHGlobal(bytes.Length + 1);
                Marshal.Copy(bytes, 0, pMrl, bytes.Length);
                Marshal.WriteByte(pMrl, bytes.Length, 0);
                return libvlc_media_new_path(libvlc_instance, pMrl);
            }
            finally
            {
                if (pMrl != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pMrl);
                }
            }
        }
        public static IntPtr libvlc_media_new_location(IntPtr libvlc_instance, string path)
        {
            IntPtr pMrl = IntPtr.Zero;
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(path);
                pMrl = Marshal.AllocHGlobal(bytes.Length + 1);
                Marshal.Copy(bytes, 0, pMrl, bytes.Length);
                Marshal.WriteByte(pMrl, bytes.Length, 0);
                return libvlc_media_new_path(libvlc_instance, pMrl);
            }
            finally
            {
                if (pMrl != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pMrl);
                }
            }
        }

        // 创建一个libvlc实例，它是引用计数的  
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        private static extern IntPtr libvlc_new(int argc, IntPtr argv);

        // 从视频来源(例如Url)构建一个libvlc_meida  
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        private static extern IntPtr libvlc_media_new_location(IntPtr libvlc_instance, IntPtr path);

        // 从本地文件路径构建一个libvlc_media  
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        private static extern IntPtr libvlc_media_new_path(IntPtr libvlc_instance, IntPtr path);

        // 播放列表
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern IntPtr libvlc_media_list_player_new(IntPtr libvlc_media_list_player);

        //释放播放实例
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_list_player_release(IntPtr libvlc_media_list_player);

        //播放list实例
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_list_player_play(IntPtr libvlc_media_list_player);

        //暂停list实例
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_list_player_pause(IntPtr libvlc_media_list_player);

        //停止list实例
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_list_player_stop(IntPtr libvlc_media_list_player);

        //播放某个视频实例
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_list_player_play_item_at_index(IntPtr libvlc_media_list_player,int index);

        //播放下个视频
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_list_player_next(IntPtr libvlc_media_list_player);

        //设置播放模式
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_list_player_set_playback_mode(IntPtr libvlc_media_list_player, int index);

        //设置播放模式
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_list_player_set_media_player(IntPtr libvlc_media_list_player, IntPtr libvlc_media_player);

        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_player_set_hwnd(IntPtr libvlc_mediaplayer, Int32 drawable);

        //添加播放列表到播放器
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_list_player_set_media_list(IntPtr libvlc_mediaplayer, IntPtr libvlc_media_list);

        //视频播放列表
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern IntPtr  libvlc_media_list_new(IntPtr libvlc_instance);

        //添加视频到播放列表
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_list_add_media(IntPtr libvlc_media_list, IntPtr libclc_media);

        //添加视频到播放列表
        [DllImport("libvlc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern IntPtr libvlc_media_list_player_get_media_player(IntPtr libvlc_media_list);

        




    }
}
