# NOTE!
## This is an old version of wavy. It uses the same language design, however doesn't compile to bytecode for a vm, instead it executes and evaluates directly. However, in this state, it does work

<p align="center">
  <img src="https://i.imgur.com/iLUQ0jh.png">
</p><br /><br />

# What is Wavy~?
Wavy~ is an intepreted, lightweight scripting language intended for use in runtime compilation in a c# enviroment

# Ease of use
Wavy~ was developed to be simple to use and learn, all you have to do to run your code in your project is create a new instance of a wavy runtime and give it your code as a string!

	WavyRuntime runtime = new WavyRuntime();
	runtime.compile(program);

Alternatively use the tilde client to repl!

# Native Customization
It is easy to create native c# extensions for your Wavy~ enviroment, your native contributions for the Wavy~ project are always welcome!
