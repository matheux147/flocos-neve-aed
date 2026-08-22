using flocos_neve_aed;
using ScottPlot;

Console.WriteLine("  PROBLEMA DOS FLOCOS DE NEVE - BENCHMARK & ANÁLISE (AED)");
Console.WriteLine();

// 1. Gera as instâncias de teste no diretório de dados
Console.WriteLine("1. Verificando e gerando instâncias de teste...");
GerarInstancias.Executar();
Console.WriteLine();

// 2. Medição dos tempos de execução para diferentes valores de N
Console.WriteLine("2. Executando benchmark das soluções...");
Console.WriteLine();

int[] tamanhosDeInstanciaN = { 500, 1000, 2000, 4000, 8000, 16000 };

double[] temposAlgoritmoIngenuo = new double[tamanhosDeInstanciaN.Length];
double[] temposAlgoritmoHash = new double[tamanhosDeInstanciaN.Length];
double[] eixoXQuantidadeFlocos = new double[tamanhosDeInstanciaN.Length];

Console.WriteLine($"{"N",8} | {"Tempo Ingênua (s)",18} | {"Tempo Hash (s)",16} | {"Speedup (x)",12}");
Console.WriteLine(new string('-', 62));

for (int i = 0; i < tamanhosDeInstanciaN.Length; i++)
{
    int quantidadeFlocos = tamanhosDeInstanciaN[i];
    eixoXQuantidadeFlocos[i] = quantidadeFlocos;
    
    string caminhoArquivoInstancia = $"instancias_flocos/instancias/floco_semgemeos_{quantidadeFlocos}.txt";

    // Mede tempo da solução ingênua (menor tempo de 3 repetições)
    var (menorTempoIngenuo, _) = Flocos.Benchmark(
        caminhoArquivoInstancia, 
        Flocos.ExisteParGemeoIngenuo, 
        repeticoes: 3
    );
    temposAlgoritmoIngenuo[i] = menorTempoIngenuo;

    // Mede tempo da solução com tabela hash (menor tempo de 3 repetições)
    var (menorTempoHash, _) = Flocos.Benchmark(
        caminhoArquivoInstancia, 
        Flocos.ExisteParGemeoHash, 
        repeticoes: 3
    );
    temposAlgoritmoHash[i] = menorTempoHash;

    // Speedup: quantas vezes a solução Hash foi mais rápida que a Ingênua
    double vezesMaisRapido = menorTempoIngenuo / menorTempoHash;

    Console.WriteLine($"{quantidadeFlocos,8} | {menorTempoIngenuo,18:F6} | {menorTempoHash,16:F6} | {vezesMaisRapido,11:F1}x");
}

Console.WriteLine();

// 3. Geração do gráfico comparativo de desempenho
Console.WriteLine("3. Gerando gráfico comparativo...");

string pastaDestinoOutput = "output";
if (!Directory.Exists(pastaDestinoOutput))
{
    Directory.CreateDirectory(pastaDestinoOutput);
}

var grafico = new Plot();

// Curva da solução ingênua
var curvaIngenua = grafico.Add.Scatter(eixoXQuantidadeFlocos, temposAlgoritmoIngenuo);
curvaIngenua.LegendText = "Solução Ingênua - O(N²)";
curvaIngenua.Color = Colors.Crimson;
curvaIngenua.LineWidth = 2.5f;
curvaIngenua.MarkerSize = 8;

// Curva da solução com tabela hash
var curvaHash = grafico.Add.Scatter(eixoXQuantidadeFlocos, temposAlgoritmoHash);
curvaHash.LegendText = "Solução Tabela Hash - O(N)";
curvaHash.Color = Colors.RoyalBlue;
curvaHash.LineWidth = 2.5f;
curvaHash.MarkerSize = 8;

// Configuração dos eixos e legenda
grafico.Title("Comparação de Desempenho: Solução Ingênua vs Tabela Hash");
grafico.XLabel("Quantidade de Flocos (N)");
grafico.YLabel("Tempo de Execução (segundos)");
grafico.ShowLegend(Alignment.UpperLeft);

// Salva a imagem gerada no disco no formato PNG (largura 850px, altura 600px)
string caminhoSalvarGrafico = Path.Combine(pastaDestinoOutput, "grafico_comparativo.png");
grafico.SavePng(caminhoSalvarGrafico, 850, 600);
Console.WriteLine($"-> Gráfico salvo com sucesso em: {caminhoSalvarGrafico}");
Console.WriteLine();

// 4. Análise de casos específicos
Console.WriteLine("4. Análise de desempenho e casos específicos...");
Console.WriteLine();

// 4.1 Fator de crescimento ao dobrar N
Console.WriteLine("-> Análise do fator de crescimento ao dobrar N (N -> 2N):");
Console.WriteLine($"{"Transição (N -> 2N)",22} | {"Fator Cresc. Ingênua",22} | {"Fator Cresc. Hash",20}");
Console.WriteLine(new string('-', 72));

for (int i = 0; i < tamanhosDeInstanciaN.Length - 1; i++)
{
    int nAtual = tamanhosDeInstanciaN[i];
    int nDobro = tamanhosDeInstanciaN[i + 1];

    double fatorCrescimentoIngenuo = temposAlgoritmoIngenuo[i + 1] / temposAlgoritmoIngenuo[i];
    double fatorCrescimentoHash = temposAlgoritmoHash[i + 1] / temposAlgoritmoHash[i];

    Console.WriteLine($"{nAtual,8} -> {nDobro,8} | {fatorCrescimentoIngenuo,19:F2}x | {fatorCrescimentoHash,17:F2}x");
}

Console.WriteLine();
Console.WriteLine("Conclusão 5a:");
Console.WriteLine("  * Solução Ingênua: O tempo cresce ~4x a cada dobro de N (Comportamento Quadrático O(N²)).");
Console.WriteLine("  * Solução Hash   : O tempo cresce ~2x a cada dobro de N (Comportamento Linear O(N)).");
Console.WriteLine();

// 4.2 Posição do par gêmeo (melhor caso vs pior caso em N = 2000)
Console.WriteLine("-> Análise da posição do par gêmeo (N = 2.000):");
string arquivoGemeoInicio = "instancias_flocos/instancias/floco_2000_gemeo_inicio.txt";
string arquivoGemeoFim = "instancias_flocos/instancias/floco_2000_gemeo_fim.txt";

var (tempoIngenuoInicio, parInicio) = Flocos.Benchmark(arquivoGemeoInicio, Flocos.ExisteParGemeoIngenuo, repeticoes: 3);
var (tempoHashInicio, _) = Flocos.Benchmark(arquivoGemeoInicio, Flocos.ExisteParGemeoHash, repeticoes: 3);

var (tempoIngenuoFim, parFim) = Flocos.Benchmark(arquivoGemeoFim, Flocos.ExisteParGemeoIngenuo, repeticoes: 3);
var (tempoHashFim, _) = Flocos.Benchmark(arquivoGemeoFim, Flocos.ExisteParGemeoHash, repeticoes: 3);

Console.WriteLine($"  * Gêmeo no Início (encontrado em {parInicio}) -> Ingênua: {tempoIngenuoInicio:F6} s | Hash: {tempoHashInicio:F6} s");
Console.WriteLine($"  * Gêmeo no Fim    (encontrado em {parFim})    -> Ingênua: {tempoIngenuoFim:F6} s | Hash: {tempoHashFim:F6} s");
Console.WriteLine();
Console.WriteLine("Conclusão 5b:");
Console.WriteLine("  * Solução Ingênua: O tempo varia com a posição do par por causa do encerramento antecipado (early-exit).");
Console.WriteLine("  * Solução Hash   : O tempo permanece estável com custo O(1) médio por consulta.");
Console.WriteLine();

// 4.3 Instância com N = 100.000
Console.WriteLine("-> Análise para N = 100.000:");
string arquivoGrande100k = "instancias_flocos/instancias/floco_grande_100000.txt";
var (tempoHash100k, _) = Flocos.Benchmark(arquivoGrande100k, Flocos.ExisteParGemeoHash, repeticoes: 3);

// Projeção baseada no tempo medido em N = 1000
double tempoBase1k = temposAlgoritmoIngenuo[1];
double tempoEstimadoIngenuo100k = tempoBase1k * Math.Pow(100000.0 / 1000.0, 2);
double vezesMaisRapido100k = tempoEstimadoIngenuo100k / tempoHash100k;

Console.WriteLine($"  * Tempo medido da Tabela Hash (N = 100.000)        : {tempoHash100k:F6} s");
Console.WriteLine($"  * Tempo estimado da Solução Ingênua (N = 100.000)   : {tempoEstimadoIngenuo100k:F2} s (~{tempoEstimadoIngenuo100k / 60.0:F1} minutos)");
Console.WriteLine($"  * Diferença relativa de desempenho (Speedup)       : {vezesMaisRapido100k:F0}x mais rápido com Tabela Hash");
Console.WriteLine();
Console.WriteLine("Conclusão 5c:");
Console.WriteLine($"  * NÃO seria razoável rodar a solução ingênua em N = 100.000, pois ela levaria cerca de {vezesMaisRapido100k:F0} vezes mais tempo");
Console.WriteLine("    que a tabela hash, comprovando a inviabilidade da complexidade O(N²) para grandes volumes de dados.");
Console.WriteLine();

  