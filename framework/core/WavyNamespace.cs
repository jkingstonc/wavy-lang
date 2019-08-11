namespace wavy.core
{
    class WavyNamespace
    {
        public string name;
        public Scope scope;

        public WavyNamespace(string name, Scope scope)
        {
            this.name = name;
            this.scope = scope;
        }

        public WavyNamespace(string name)
        {
            this.name = name;
            this.scope = new Scope();
        }
    }
}
