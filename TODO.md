# TODO list

# New Features
- allow compound assignment and ++ to work on non-variables such as functions and this
- allow for nested namespaces
- lambdas
- escape sequences
- interpreter memory settings
- literal dictionaries
- different numeric types, e.g. 1,234, 0x123, 0b123
- importing object based e.g. import a.b.c (a is a folder class, with a method 'b' which imports b)
- module manager, e.g. way to install custom modules and bundle custom modules
- plugin Code Structure [i.e. plugin lexers & parsers]

# Tweaks

- Clean ways of getting/setting & creating objects in code [and validation]
- possibly remove scope resolver?

# Optimization Features
- method caching : Cache method lookups into a Dict<string, callable> to see if we have cached the method
- WObject caching in loops: Cache the object and the method we want to call in the loop for faster lookup
- static garbage collection during resolving
- runtime garbage collection
