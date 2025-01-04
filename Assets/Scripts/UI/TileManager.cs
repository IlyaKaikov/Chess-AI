namespace ChessAI.UI
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Runtime.InteropServices.WindowsRuntime;
    using UnityEngine.InputSystem;

    public class TileManager : MonoBehaviour
    {
        public GameObject tilePrefab;
        public GameObject arrowPrefab;
        private readonly Dictionary<Vector2Int, GameObject> lastMoveOverlay = new();
        private readonly Dictionary<Vector2Int, GameObject> moveOptionsOverlay = new();
        private readonly Dictionary<Vector2Int, GameObject> markedTilesOverlay = new();
        private readonly Dictionary<(Vector2Int from, Vector2Int to), GameObject> arrows = new();

        public void GenerateChessboard(int boardSize,Color brightColor, Color darkColor)
        {
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    GameObject tile = Instantiate(tilePrefab, transform);
                    tile.transform.position = new Vector3(x, y, 0);
                    tile.GetComponent<SpriteRenderer>().color = (x + y) % 2 == 0 ? darkColor : brightColor;
                }
            }
        }

        public void HighlightLastMove(Vector2Int from, Vector2Int to)
        {
            ClearLastMoveHighlights();

            CreateOverlay(from, new Color(0.3f, 0.9f, 0.3f, 0.35f), true);
            CreateOverlay(to, new Color(0.3f, 0.9f, 0.3f, 0.35f), true);
        }

        public void HighlightMoveOptions(List<Vector2Int> positions)
        {
            ClearMoveOptionsHighlights();

            foreach (var position in positions)
            {
                CreateOverlay(position, new Color(0f, 0.8f, 1f, 0.35f), false);
            }
        }

        private void CreateOverlay(Vector2Int position, Color color, bool isGreen)
        {
            GameObject overlay = Instantiate(tilePrefab, transform);
            overlay.transform.position = new Vector3(position.x, position.y, 0);
            overlay.GetComponent<SpriteRenderer>().color = color;
            if (isGreen)
            {
                overlay.GetComponent<SpriteRenderer>().sortingOrder = 1;
                lastMoveOverlay[position] = overlay;
            }
            else
            {
                overlay.GetComponent<SpriteRenderer>().sortingOrder = 2;
                moveOptionsOverlay[position] = overlay;
            }
        }

        public void ClearLastMoveHighlights()
        {
            foreach (var overlay in lastMoveOverlay.Values)
            {
                Destroy(overlay);
            }
            lastMoveOverlay.Clear();
        }

        public void ClearMoveOptionsHighlights()
        {
            foreach (var overlay in moveOptionsOverlay.Values)
            {
                Destroy(overlay);
            }
            moveOptionsOverlay.Clear();
        }

        public void ToggleMarkedTiles(Vector2Int position)
        {
            if (markedTilesOverlay.ContainsKey(position))
            {
                Destroy(markedTilesOverlay[position]);
                markedTilesOverlay.Remove(position);
            }
            else
            {
                GameObject highlight = Instantiate(tilePrefab, transform);
                highlight.transform.position = new Vector3(position.x, position.y, 0);
                highlight.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f, 0.5f);
                markedTilesOverlay[position] = highlight;
                highlight.GetComponent<SpriteRenderer>().sortingOrder = 2;
            }
        }

        public void ToggleArrow(Vector2Int from, Vector2Int to)
        {
            var key = (from, to);
            if (arrows.ContainsKey(key))
            {
                RemoveArrow(from, to);
                return;
            }
            CreateArrow(from, to);
        }

        public void CreateArrow(Vector2Int from, Vector2Int to)
        {
            var key = (from, to);
            if (arrows.ContainsKey(key)) return;

            GameObject arrow = Instantiate(arrowPrefab, transform);
            Vector2 direction = new(to.x - from.x, to.y - from.y);
            float distance = direction.magnitude;
            arrow.transform.SetPositionAndRotation(new Vector3(from.x, from.y, 0), Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));

            Transform body = arrow.transform.Find("Body");
            if (body != null)
            {
                body.localScale = new Vector3(distance, body.localScale.y, body.localScale.z);
                body.localPosition = new Vector3(distance / 2f, 0, 0);
            }

            Transform head = arrow.transform.Find("Head");
            if (head != null)
            {
                head.localPosition = new Vector3(distance, 0, 0);
            }
            arrows[key] = arrow;
        }

        public void RemoveArrow(Vector2Int from, Vector2Int to)
        {
            var key = (from, to);
            if (arrows.ContainsKey(key))
            {
                Destroy(arrows[key]);
                arrows.Remove(key);
                return;
            }
        }

        public bool ArrowExists(Vector2Int from, Vector2Int to)
        {
            var key = (from, to);
            return arrows.ContainsKey(key);
        }

        public void ClearMarkedTilesAndArrows()
        {
            foreach (var highlight in markedTilesOverlay.Values)
            {
                Destroy(highlight);
            }
            markedTilesOverlay.Clear();

            foreach (var arrow in arrows.Values)
            {
                Destroy(arrow);
            }
            arrows.Clear();
        }
    }
}
