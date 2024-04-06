using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    /// <summary>
    /// 控制台程序的参数解析类库CommandLine简单使用说明
    /// Install-Package CommandLineParser
    /// </summary>
    internal class Program
    {
        //输出信息时的头信息
        private static readonly HeadingInfo HeadingInfo = new HeadingInfo("演示程序", "V1.8");

        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args)
              .WithParsed(RunOptions)
              .WithNotParsed(HandleParseError);

            ////这种输出会在前面添加"演示程序"几个字
            //HeadingInfo.WriteError("包含头信息的错误数据");
            //HeadingInfo.WriteMessage("包含头信息的消息数据");

            //Console.WriteLine("不包含头信息的错误数据");
            //Console.WriteLine("不包含头信息的消息数据");

            //var options = new Options();
            //if (CommandLine.Parser.Default.ParseArguments(args, options))
            //{
            //    Console.WriteLine("Input File:" + options.InputFile);
            //    Console.WriteLine("Output File:" + options.OutputFile);

            //    Console.WriteLine("开始时间:" + options.StartTime.ToString("yyyy年MM月dd日 HH点mm分"));
            //    Console.WriteLine("结束时间:" + options.EndTime.ToString("yyyy年MM月dd日 HH点mm分"));
            //    Console.Read();
            //}
            //else
            //{
            //    Console.WriteLine(options.GetUsage());
            //    Console.Read();
            //}

         //   {
         //       var text = "subCommand file=filename.txt verbose simulate";
         //       var args = text.Split();
         //       var result = CommandLine.Parser.Default.ParseArguments<Options>(args).MapResult((opts) => RunOptionsAndReturnExitCode(opts), //in case parser sucess
         //errs => HandleParseError(errs)); //in  case parser fail
         //       Console.WriteLine("Return code= {0}", result);
         //   }
        }

        static void RunOptions(Options opts)
        {
            //handle options
        }
        static void HandleParseError(IEnumerable<Error> errs)
        {
            //handle errors
        }


        ////3)	//In sucess: the main logic to handle the options
        //static int RunOptionsAndReturnExitCode(Options o)
        //{
        //    Console.WriteLine("Success");
        //    var exitCode = 0;
        //    var props = o.Props;
        //    //foreach (var prop in props)
        //    Console.WriteLine("props= {0}", string.Join(",", props));
        //    return exitCode;
        //}

        ////in case of errors or --help or --version
        //static int HandleParseError(IEnumerable<Error> errs)
        //{
        //    var result = -2;
        //    Console.WriteLine("errors {0}", errs.Count());
        //    if (errs.Any(x => x is HelpRequestedError || x is VersionRequestedError))
        //        result = -1;
        //    Console.WriteLine("Exit code {0}", result);
        //    return result;
        //}

        //class Options
        //{
        //    [Value(0)]
        //    public IEnumerable<string> Props
        //    {
        //        get;
        //        set;
        //    }
        //}


        class Options
        {
            [Option('r', "read", Required = true, HelpText = "Input files to be processed.")]
            public IEnumerable<string> InputFiles { get; set; }

            // Omitting long name, defaults to name of property, ie "--verbose"
            [Option(
              Default = false,
              HelpText = "Prints all messages to standard output.")]
            public bool Verbose { get; set; }

            [Option("stdin",
              Default = false,
              HelpText = "Read from stdin")]
            public bool stdin { get; set; }

            [Value(0, MetaName = "offset", HelpText = "File offset.")]
            public long? Offset { get; set; }
        }

        //class Options
        //{
        //    [Option('r', "read", MetaValue = "FILE", Required = true, HelpText = "输入数据文件")]
        //    public string InputFile { get; set; }

        //    [Option('w', "write", MetaValue = "FILE", Required = false, HelpText = "输出数据文件")]
        //    public string OutputFile { get; set; }


        //    [Option('s', "start-time", MetaValue = "STARTTIME", Required = true, HelpText = "开始时间")]
        //    public DateTime StartTime { get; set; }

        //    [Option('e', "end-time", MetaValue = "ENDTIME", Required = true, HelpText = "结束时间")]
        //    public DateTime EndTime { get; set; }


        //    //[HelpOption]
        //    //public string GetUsage()
        //    //{
        //    //    return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        //    //}

        //}
    }


}
