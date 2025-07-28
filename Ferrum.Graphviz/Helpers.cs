using System.Text;

namespace Ferrum.Graphviz;

internal static class Helpers
{
    public static (IEnumerable<T>, IEnumerable<(T, T)>) MappedViews<TNode, T>(
        Func<TNode, T> mapper,
        Func<TNode, IEnumerable<TNode>> children,
        TNode root)
    {
        var targets = new Queue<(TNode, T)>();
        var values = new List<T>();
        var edges = new List<(T, T)>();
        targets.Enqueue((root, mapper(root)));
        while (targets.Count > 0)
        {
            var (node, value) = targets.Dequeue();
            values.Add(value);
            foreach (var child in children(node))
            {
                var childValue = mapper(child);
                edges.Add((value, childValue));
                targets.Enqueue((child, childValue));
            }
        }
        return (values, edges);
    }

    public static string FormatTree<TNode>(
        Func<TNode, string> view,
        Func<TNode, IEnumerable<TNode>> children,
        TNode root)
    {
        var viewsCount = new Dictionary<string, int>();
        string GetViewWithCount(TNode node)
        {
            var viewStr = view(node);
            var usages = viewsCount.GetValueOrDefault(viewStr, 0) + 1;
            viewsCount[viewStr] = usages;
            var viewStrEscaped = viewStr.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");
            var viewStrEscapedWithCount = usages <= 1 ? $"\"{viewStrEscaped}\"" : $"\"{viewStrEscaped} ({usages})\"";
            return viewStrEscapedWithCount;
        }
        var (nodes, edges) = MappedViews(GetViewWithCount, children, root);

        var sb = new StringBuilder();
        sb.Append("digraph ErrorGraph {").AppendLine();
        foreach (var node in nodes)
        {
            sb.Append("  ").Append(node).Append(';').AppendLine();
        }
        foreach (var (nodeFrom, nodeTo) in edges)
        {
            sb.Append("  ").Append(nodeFrom).Append(" -> ").Append(nodeTo).Append(';').AppendLine();
        }
        sb.Append('}').AppendLine();
        return sb.ToString();
    }
}