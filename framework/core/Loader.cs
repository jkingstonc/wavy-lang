using System.Collections.Generic;
using System.IO;
using System.Text;

public class Loader
{
    public static string wavy_env = ""; // YOUR PATH HERE
    // Path for native library
    public static string native_path = wavy_env + "nativeobj\\";
    // Path for all import modules
    public static string modules_path = wavy_env + "modules\\";
    // Path for the std package inside the modules directory
    public static string builtin_path = modules_path + "builtin\\";

    private Interpreter interpreter;
    private List<string> loaded_modules = new List<string>();

    public Loader(Interpreter interpreter)
    {
        this.interpreter = interpreter;
    }

    // Load the standard library to the interpreter
    public void load_builtin_lib()
    {   
        import("builtin");
    }

    // Import a module, with a given path to a directory, or specific file
    public void import(string module)
    {
        // Replace the .'s with path seperators
        module = module.Replace(".", "\\");
        string search = modules_path + module;
        // First check the modules path
        if (Directory.Exists(search))
        {
            import_path(search);
            return;
        }else if (File.Exists(search + ".w~"))
        {
            import_module(search + ".w~");
            return;
        }
        // Else, check the local files to see if it is a custom module
        string custom_search = Directory.GetCurrentDirectory() + module;
        if (Directory.Exists(custom_search))
        {
            import_path(custom_search);
            return;
        }
        else if (File.Exists(custom_search + ".w~"))
        {
            import_module(custom_search + ".w~");
            return;
        }
        throw new RuntimeException("Couldn't find module '"+module+"'");
    }

    // Import a given module file
    private void import_module(string file)
    {
        // Check if we haven't already loaded it
        if (!loaded_modules.Contains(file))
        {
            loaded_modules.Add(file);
            string contents = File.ReadAllText(file, Encoding.UTF8);
            compile(contents);
        }
    }

    // Recursively import all modules in a directory
    private void import_path(string directory_path)
    {
        var paths = Directory.GetDirectories(directory_path);
        var files = Directory.GetFiles(directory_path);
        foreach(string path in paths)
        {
            import_path(path);
        }
        foreach (string file in files)
        {
            import_module(file);
        }
    }

    // Compile a module to the interpreter
    public void compile(string code)
    {
        Lexer lexer = new Lexer();
        Parser parser = new Parser();
        ScopeResolver scope_resolver = new ScopeResolver(this.interpreter);
        List<Token> tokens = lexer.lex(code);
        List<Statement> statements = parser.parse(tokens);
        scope_resolver.resolve(statements);
        this.interpreter.interpret(statements);
    }
}