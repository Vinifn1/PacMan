using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GhostChase : GhostBehavior
{
    public enum AlgorithmType
    {
        Astar,
        Greedy,
    };
    public List<AstarNode> openList;
    public List<AstarNode> closedList;
    
    public AlgorithmType algorithmType;
    private void OnDisable()
    {
        var redMarkers = GameObject.FindGameObjectsWithTag("RedMarker");
        foreach (var redMarker in redMarkers)
        {
            Destroy(redMarker);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();
        // Chase Behavior = Segue o Pacman utilizando algum algoritmo de busca heuristica
        if (node != null && this.enabled)
        {
            var position = other.transform.position; // Posicao atual
            
            // Result = lista ordenada das posicoes encontradas pelos algoritmos. Guloso ou A*.
            // O algoritmo retorna um vetor de posicoes que o fantasma deve seguir
            var result = algorithmType == AlgorithmType.Greedy ? GreedySearch(position) : AstarSearch(position);

            Vector2 nextDirection;
            
            if (result == null || result.Count == 0) // regra apenas para quando Fantasma quiser ficar parado pelo algoritmo guloso.
            {                                        // nesse caso, seta uma direcao aleatoria
                var possibleStates = GetPossibleStates(position);
                var randomState = possibleStates[Random.Range(0, possibleStates.Count)];
                nextDirection = new Vector2(randomState.x - position.x, randomState.y - position.y);
            }
            else  // Regra principal : seta a proxima direcao com base na lista de casas que os algoritmos de buscas retornam.
            {     // pega a primeira casa que o algoritmo decidiu ir, e calcula seu vetor de direcao.
                nextDirection = new Vector2(result[0].x - position.x, result[0].y - position.y);
            }

            this.Ghost.Movement.SetDirection(nextDirection); // Seta a direção que o fantasma vai se mover.
            
            var lastResult = result.Last(); // variavel utilizada para Printar o marcador de casa escolhida pelo algoritmo.

            if (algorithmType == AlgorithmType.Astar) // se for algoritmo A*, marcador amarelo
            {
                var yellowMarkers = GameObject.FindGameObjectsWithTag("YellowMarker"); // procura as marcacoes antigas
                foreach (var yellowMarker in yellowMarkers)                             
                {
                    Destroy(yellowMarker);                                                       // e deleta
                }
                Instantiate(this.Ghost.yellowMarker, new Vector3(lastResult.x, lastResult.y, -5), Quaternion.identity); // cria uma nova marcaçao
            }

            if (algorithmType == AlgorithmType.Greedy) // se for algoritmo Guloso, marcador vermelho
            {
                var redMarkers = GameObject.FindGameObjectsWithTag("RedMarker");
                foreach (var redMarker in redMarkers)
                {
                    Destroy(redMarker);
                }
                Instantiate(this.Ghost.redMarker, new Vector3(lastResult.x, lastResult.y, -5), Quaternion.identity);
            }
        }
    }

    private List<Vector2> AstarSearch(Vector2 initialPosition)
    {
        var targetPosition = GetTargetNodePosition();

        var initialNodeCost = CalculateEuclidianDistanceCost(initialPosition, this.GetTargetNodePosition());
        Debug.Log("initialNodecossst = " + initialNodeCost);
        AstarNode startNode = new AstarNode()
        {
            CameFromNode = null,
            HCost = initialNodeCost,
            GCost = 0, // Valor de G inicial
            Position = initialPosition,
        };
        
        openList = new List<AstarNode>();
        closedList = new List<AstarNode>();

        openList.Add(startNode);
        int maxIterations = 1000;
        int countIterations = 0;
        Debug.Log("Targetposition = " + targetPosition);
        while (openList.Count > 0 && countIterations < maxIterations)
        {
            countIterations++;
            AstarNode currentNode = GetLowestFCostNode(openList);
            if (currentNode.Position == targetPosition)
            {
                Debug.Log("Fim");
                return CalculatePath(currentNode).Select(c => c.Position).ToList();
            }
            
            var vizinhos = GetPossibleStates(currentNode.Position);
            Debug.Log("Qtd de vizinhos = " + vizinhos.Count);

            foreach (var vizinho in vizinhos)
            {
                // Calculas os custos desse vizinho
                AstarNode nodeVizinho = new AstarNode()
                {
                    CameFromNode = currentNode,
                    Position = vizinho,
                    GCost = currentNode.GCost + 1, 
                    HCost = CalculateEuclidianDistanceCost(vizinho, GetTargetNodePosition()),
                };

                // Verifica se esse NÓ que está sendo calculado já está na lista fechada, se já estiver, ignora e vai pro proximo caso.
                if (closedList.Any(n => n.Position == nodeVizinho.Position))
                {
                    continue;
                }
                
                // Verifica se esse NÓ que está sendo calculado já está na lista aberta, se não estiver, adiciona.
                if (!openList.Any(n => n.Position == nodeVizinho.Position))
                    openList.Add(nodeVizinho);

            }
            
            // Tiro o nó que já calculei todos os vizinhos da lista aberta e mando pra fechada.
            openList.Remove(currentNode);
            closedList.Add(currentNode);
        }
        
        return new List<Vector2>();
    }

    private List<AstarNode> CalculatePath(AstarNode endNode)
    {
        List<AstarNode> path = new List<AstarNode>();
        path.Add(endNode);
        AstarNode currentNode = endNode;
        while (currentNode.CameFromNode != null)
        {
            path.Add(currentNode.CameFromNode);
            currentNode = currentNode.CameFromNode;
        }

        path.Remove(path.Last());
        path.Reverse();
        return path;
    }
    
    
    private List<Vector2> GreedySearch(Vector2 initialPosition)
    {
        int maxIterations = 300;
        int iterations = 0;
        List<Vector2> resultsPositions = new List<Vector2>();
        var currentState = initialPosition;
        var currentCost = CalculateEuclidianDistanceCost(initialPosition, GetTargetNodePosition());
        
        while (iterations <= maxIterations)
        {
            iterations++;
            
            if (currentState == GetTargetNodePosition()) // Primeiro passo, verificar se encontrou o Pacman
            {                                            // e retorno a lista de passos calculados ate o momento.
                return resultsPositions;
            }
            
            var vizinhos = GetPossibleStates(currentState); // Pega as posicoes dos vizinhos

            if (vizinhos.Count == 0) // Se nao encontrar nenhum vizinho, finaliza a busca.
                return resultsPositions;
            
            List<PositionAndCost> statesWithCosts = new List<PositionAndCost>();
            
            foreach (var vizinho in vizinhos) // Calcula o custo e adiciona cada vizinho na lista.
            {
                statesWithCosts.Add(new PositionAndCost()
                {
                    Cost = CalculateEuclidianDistanceCost(vizinho, GetTargetNodePosition()),
                    Position = vizinho
                });
            }

            // Ordena a lista de vizinhos em ordem ascendente, por custo. Aí pega o primeiro que vai ser o menor valor de custo
            var bestState = statesWithCosts.OrderBy(p => p.Cost).FirstOrDefault();
            
            // Se todos os vizinhos tiverem Custo maior do que a posicao atual (Minimo local)
            // finalizo a busca e retorno a lista de posicoes
            if (bestState.Cost >= currentCost) 
            {
                return resultsPositions;
            }
            
            // Senao, adiciono a posicao com o menor custo na lista, considero ela como o passo atual, e itera novamente
            resultsPositions.Add(bestState.Position);
            currentState = bestState.Position;
            currentCost = bestState.Cost;
        }

        return resultsPositions;
    }
    
    private List<Vector2> GetPossibleStates(Vector2 position)
    {
        List<Vector2> possibleStates = new List<Vector2>();
        
        RaycastHit2D hitRight = Physics2D.BoxCast(position, Vector2.one * 0.75f, 0.0f, Vector2.right, 1.0f, this.Ghost.obstacleLayer);
        RaycastHit2D hitLeft = Physics2D.BoxCast(position, Vector2.one * 0.75f, 0.0f, Vector2.left, 1.0f, this.Ghost.obstacleLayer);
        RaycastHit2D hitUp = Physics2D.BoxCast(position, Vector2.one * 0.75f, 0.0f, Vector2.up, 1.0f, this.Ghost.obstacleLayer);
        RaycastHit2D hitDown = Physics2D.BoxCast(position, Vector2.one * 0.75f, 0.0f, Vector2.down, 1.0f, this.Ghost.obstacleLayer);
        
        if (hitRight.collider == null)
            possibleStates.Add(position + Vector2.right);

        if (hitLeft.collider == null)
            possibleStates.Add(position + Vector2.left);

        if (hitUp.collider == null)
            possibleStates.Add(position + Vector2.up);

        if (hitDown.collider == null)
            possibleStates.Add(position + Vector2.down);

        if (possibleStates.Count == 1)
        {
            Debug.Log("erro no raycast : ");
            
            Debug.DrawRay(position, Vector2.up, hitUp.collider != null ? Color.red : Color.green, 10.0f);
            
            Debug.DrawRay(position, Vector2.down, hitDown.collider != null ? Color.red : Color.green, 10.0f);
            
            Debug.DrawRay(position, Vector2.left, hitLeft.collider != null ? Color.red : Color.green, 10.0f);
            
            Debug.DrawRay(position, Vector2.right, hitRight.collider != null ? Color.red : Color.green, 10.0f);
        }
        return possibleStates;
    }
    
    private float CalculateEuclidianDistanceCost(Vector2 position, Vector2 target)
    {
        float cost = Math.Abs(Vector2.Distance(position, target));

        return cost;
    }

    private AstarNode GetLowestFCostNode(List<AstarNode> astarNodeList)
    {
        AstarNode lowest = astarNodeList[0];
        for (int i = 1; i < astarNodeList.Count(); i++)
        {
            if (astarNodeList[i].FCost < lowest.FCost)
            {
                lowest = astarNodeList[i];
            }
        }

        Debug.Log("Lowest F cost = " + lowest.FCost);
        return lowest;
    }
    
    public Vector2 GetTargetNodePosition()
    {
        var position = this.Ghost.target.position;
        
        int signalX = position.x >= 0 ? 1 : -1;
        int signalY = position.y >= 0 ? 1 : -1;
        
        var absoluteX = Math.Abs(position.x);
        var absoluteY = Math.Abs(position.y);
        
        int integerX = (int)Math.Truncate(absoluteX);
        int integerY = (int)Math.Truncate(absoluteY);

        return new Vector2((integerX + 0.5f) * signalX, (integerY + 0.5f) * signalY);
    }
}

public class PositionAndCost
{
    public Vector2 Position { get; set; }
    public float Cost { get; set; }
}

public class AstarNode
{
    public Vector2 Position { get; set; }
    public float GCost { get; set; } // Cost from start
    public float HCost { get; set; } // Heuristic value
    public float FCost => HCost + GCost;
    
    public AstarNode CameFromNode { get; set; }
}