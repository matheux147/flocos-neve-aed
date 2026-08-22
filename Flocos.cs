using System.Diagnostics;

namespace flocos_neve_aed;

public static class Flocos
{
    /// <summary>
    /// Lê um arquivo de instância e retorna a lista de flocos.
    /// </summary>
    public static List<int[]> LeInstancia(string caminho)
    {
        string[] linhas = File.ReadAllLines(caminho);
        int n = int.Parse(linhas[0]);

        List<int[]> flocos = new List<int[]>();

        for (int i = 0; i < n; i++)
        {
            string[] partes = linhas[i + 1].Split(' ', StringSplitOptions.RemoveEmptyEntries);

            int[] floco = new int[6];
            for (int j = 0; j < 6; j++)
            {
                floco[j] = int.Parse(partes[j]);
            }

            flocos.Add(floco);
        }

        return flocos;
    }

    /// <summary>
    /// Retorna True se o floco b for alguma das 6 rotações possíveis do floco a.
    /// </summary>
    public static bool SaoGemeos(int[] a, int[] b)
    {
        for (int i = 0; i < 6; i++)
        {
            bool saoIguais = true;
            for (int j = 0; j < 6; j++)
            {
                if (a[(i + j) % 6] != b[j])
                {
                    saoIguais = false;
                    break;
                }
            }

            if (saoIguais)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Solução ingênua: compara todos os pares (i, j) da lista.
    /// </summary>
    public static (int, int)? ExisteParGemeoIngenuo(List<int[]> flocos)
    {
        int n = flocos.Count;
        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                if (SaoGemeos(flocos[i], flocos[j]))
                {
                    return (i, j);
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Retorna a menor rotação (em ordem lexicográfica) dentre as 6 rotações do floco.
    /// </summary>
    public static (int, int, int, int, int, int) ChaveCanonica(int[] floco)
    {
        var menor = ObterTupla(floco, 0);

        for (int i = 1; i < 6; i++)
        {
            var candidata = ObterTupla(floco, i);
            if (candidata.CompareTo(menor) < 0)
            {
                menor = candidata;
            }
        }

        return menor;
    }

    /// <summary>
    /// Converte a rotação a partir do índice de início em uma tupla.
    /// </summary>
    private static (int, int, int, int, int, int) ObterTupla(int[] floco, int inicio)
    {
        return (
            floco[inicio],
            floco[(inicio + 1) % 6],
            floco[(inicio + 2) % 6],
            floco[(inicio + 3) % 6],
            floco[(inicio + 4) % 6],
            floco[(inicio + 5) % 6]
        );
    }

    /// <summary>
    /// Solução com tabela hash usando dicionário para guardar as chaves já vistas.
    /// </summary>
    public static (int, int)? ExisteParGemeoHash(List<int[]> flocos)
    {
        // Guarda a chave canônica (tupla) de cada floco e seu respectivo índice
        var visto = new Dictionary<(int, int, int, int, int, int), int>();

        for (int j = 0; j < flocos.Count; j++)
        {
            var chave = ChaveCanonica(flocos[j]);

            // Se a chave já apareceu antes, encontramos o par gêmeo
            if (visto.TryGetValue(chave, out int i))
            {
                return (i, j);
            }

            // Salva a chave com o índice atual
            visto[chave] = j;
        }

        return null;
    }

    /// <summary>
    /// Mede o tempo de execução do algoritmo (retorna o menor tempo de 3 repetições).
    /// </summary>
    public static (double tempo, (int, int)? resultado) Benchmark(
        string caminho,
        Func<List<int[]>, (int, int)?> algoritmo,
        int repeticoes = 3)
    {
        List<int[]> flocos = LeInstancia(caminho);
        List<double> tempos = new List<double>();
        (int, int)? resultado = null;

        for (int i = 0; i < repeticoes; i++)
        {
            Stopwatch cronometro = Stopwatch.StartNew();
            resultado = algoritmo(flocos);
            cronometro.Stop();
            tempos.Add(cronometro.Elapsed.TotalSeconds);
        }

        double menorTempo = tempos[0];
        foreach (double tempo in tempos)
        {
            if (tempo < menorTempo)
            {
                menorTempo = tempo;
            }
        }

        return (menorTempo, resultado);
    }
}
