namespace flocos_neve_aed;

/// <summary>
/// Gerador de arquivos de instâncias de teste para o problema dos flocos de neve.
/// </summary>
public static class GerarInstancias
{
    private static readonly string OUT = Path.Combine("instancias_flocos", "instancias");

    /// <summary>
    /// Gera todos os conjuntos de instâncias de teste necessários.
    /// </summary>
    public static void Executar()
    {
        if (!Directory.Exists(OUT))
        {
            Directory.CreateDirectory(OUT);
        }

        // 1. Instância pequena de teste com par gêmeo conhecido nas posições 2 e 9
        var flocosDebug = InstanciaComGemeo(12, seed: 1, posA: 2, posB: 9);
        Escreve(Path.Combine(OUT, "floco_debug_12.txt"), flocosDebug);

        // 2. Conjunto de instâncias sem pares gêmeos com valores crescentes de N
        int[] tamanhos = { 500, 1000, 2000, 4000, 8000, 16000 };
        foreach (int n in tamanhos)
        {
            var flocosSem = InstanciaSemGemeos(n, seed: 100 + n);
            Escreve(Path.Combine(OUT, $"floco_semgemeos_{n}.txt"), flocosSem);
        }

        // 3. Instâncias com N = 2000 contendo par gêmeo no início e no final da coleção
        var flocosInicio = InstanciaComGemeo(2000, seed: 7, posA: 0, posB: 3);
        Escreve(Path.Combine(OUT, "floco_2000_gemeo_inicio.txt"), flocosInicio);

        var flocosFim = InstanciaComGemeo(2000, seed: 7, posA: 1990, posB: 1999);
        Escreve(Path.Combine(OUT, "floco_2000_gemeo_fim.txt"), flocosFim);

        // 4. Instância de grande porte para teste com tabela hash (N = 100.000)
        var flocosGrande = InstanciaSemGemeos(100000, seed: 999);
        Escreve(Path.Combine(OUT, "floco_grande_100000.txt"), flocosGrande);

        Console.WriteLine("Arquivos de teste verificados/gerados:");
        foreach (var f in Directory.GetFiles(OUT, "*.txt").OrderBy(x => x))
        {
            var info = new FileInfo(f);
            Console.WriteLine($"  {info.Name,-35} {info.Length,10} bytes");
        }
    }

    /// <summary>
    /// Gera um floco com 6 pontas com valores aleatórios entre 0 e 10 milhões.
    /// </summary>
    private static int[] FlocoAleatorio(Random rng)
    {
        int[] floco = new int[6];
        for (int i = 0; i < 6; i++)
        {
            floco[i] = rng.Next(0, 10_000_000);
        }
        return floco;
    }

    /// <summary>
    /// Desloca ciclicamente as pontas do floco k posições para a direita.
    /// </summary>
    private static int[] Rotaciona(int[] s, int k)
    {
        int[] r = new int[6];
        for (int i = 0; i < 6; i++)
        {
            r[i] = s[(i + k) % 6];
        }
        return r;
    }

    /// <summary>
    /// Inverte a ordem das pontas do floco.
    /// </summary>
    private static int[] Reflete(int[] s)
    {
        return s.Reverse().ToArray();
    }

    /// <summary>
    /// Escreve a coleção de flocos em arquivo no formato esperado: N na primeira linha e cada floco por linha.
    /// </summary>
    private static void Escreve(string caminho, List<int[]> flocos)
    {
        using (var writer = new StreamWriter(caminho))
        {
            writer.WriteLine(flocos.Count);
            foreach (var floco in flocos)
            {
                writer.WriteLine(string.Join(" ", floco));
            }
        }
    }

    /// <summary>
    /// Gera N flocos aleatórios usando semente determinística.
    /// </summary>
    private static List<int[]> InstanciaSemGemeos(int n, int seed)
    {
        var rng = new Random(seed);
        var flocos = new List<int[]>();
        for (int i = 0; i < n; i++)
        {
            flocos.Add(FlocoAleatorio(rng));
        }
        return flocos;
    }

    /// <summary>
    /// Gera N flocos e insere propositalmente um par gêmeo nas posições posA e posB.
    /// </summary>
    private static List<int[]> InstanciaComGemeo(int n, int seed, int posA, int posB, bool usaReflexao = false)
    {
        var rng = new Random(seed);
        var flocos = new List<int[]>();
        
        for (int i = 0; i < n; i++)
        {
            flocos.Add(FlocoAleatorio(rng));
        }

        var baseFloco = flocos[posA];
        int k = rng.Next(1, 5);
        var gemeo = Rotaciona(baseFloco, k);
        if (usaReflexao)
        {
            gemeo = Reflete(gemeo);
        }
        
        flocos[posB] = gemeo;
        return flocos;
    }
}
