using System;
using System.Web.Mvc;

namespace MatrixApp.Controllers
{
    public class MatrixController : Controller
    {
        // GET: Matrix
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateMatrices(string size)
        {
            if (string.IsNullOrEmpty(size))
            {
                // Если размер не выбран, возвращаем на главную страницу
                return RedirectToAction("Index");
            }

            var dimensions = size.Split('x');
            int rows = int.Parse(dimensions[0]);
            int cols = int.Parse(dimensions[1]);

            ViewBag.Rows = rows;
            ViewBag.Cols = cols;

            return View("Input");
        }

        [HttpPost]
        public ActionResult Calculate(int[][] matrixA, int[][] matrixB, string operation)
        {
            if (matrixA == null || matrixB == null || matrixA.Length == 0 || matrixB.Length == 0)
            {
                // Если данные не переданы, возвращаем на страницу ввода
                return RedirectToAction("CreateMatrices", new { size = "3x3" });
            }

            int rows = matrixA.Length;
            int cols = matrixA[0].Length;
            int[][] resultMatrix = new int[rows][];

            for (int i = 0; i < rows; i++)
            {
                resultMatrix[i] = new int[cols];
                for (int j = 0; j < cols; j++)
                {
                    if (operation == "Сложить")
                    {
                        resultMatrix[i][j] = matrixA[i][j] + matrixB[i][j];
                    }
                    else if (operation == "Умножить")
                    {
                        for (int k = 0; k < cols; k++)
                        {
                            resultMatrix[i][j] += matrixA[i][k] * matrixB[k][j];
                        }
                    }
                }
            }

            ViewBag.MatrixA = matrixA;
            ViewBag.MatrixB = matrixB;
            ViewBag.ResultMatrix = resultMatrix;
            ViewBag.Operation = operation;

            return View("Result");
        }
    }
}
