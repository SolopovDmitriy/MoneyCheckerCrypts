using MoneyChecker.Models;
using SQLiteORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyChecker.ViewModels
{
    public class CategoryViewModel
    {
        private SQLiteDBEngine _SQLiteDBEngine; // движок, через него происходят все операции с базой данных
        private SQLiteTable _categoriesTable; // объект для связи с таблицей categories в базе данных
        private List<Category> _сategories; 
        public CategoryViewModel(SQLiteDBEngine SQLiteDBEngine)
        {

            _SQLiteDBEngine = SQLiteDBEngine;
            _categoriesTable = _SQLiteDBEngine["categories"]; // получаем из движка объект SQLiteTable _categories для работы с таблицей categories

            _сategories = new List<Category>(); // создаём пустой список List<Category> для считывания данных из таблицы categories
            foreach (var row in _categoriesTable.BodyRows) // проходим по всем строкам таблицы categories, var row - один рядок в таблице
            {
                int id = Convert.ToInt32(row.Key); // в ключе объекта row хранится первичный ключ id 
                string name = row.Value[0]; // в значении объекта row список строк, в котором хранятся остальные столбцы (кроме первичного ключа id)
                int? parentId = null; // если не получится сконвертировать row.Value[1] в число, то в int? parentId должно хранится null
                if (row.Value[1] != "") // если не пустое, значит в row.Value[1] хранится число в виде строки (например, "3") которое нужно сконвертировать в число 3
                {
                    parentId = Convert.ToInt32(row.Value[1]); // конвертируем строку row.Value[1] в число
                }
                _сategories.Add(new Category { Id = id, Categ = name, Parent_Id = parentId }); // создаем объект класса Category и добавляем этот объект в список сategoriesFromDB
            }
        }
        public List<Category> DataSource {
            get { return GoDownRecursive(null); } 
        } 

        private List<Category> GoDownRecursive(int? parentId) // получить список категорий, у которых родитель = parentId
        {
            List<Category> res = new List<Category>(); // создаём пустой список
            foreach (Category subcategory in _сategories.Where(c => c.Parent_Id == parentId)) // сategoriesFromDB.Where(c => c.ParentId == parentId) ищет в списке сategoriesFromDB все категории у которых родитель == parentId
            {
                res.Add(subcategory); // добавляем в список категорию
                subcategory.SubCategories = new List<Category>(); // создаём пустой список для подкатегорий 
                subcategory.SubCategories.AddRange(GoDownRecursive(subcategory.Id)); // рекурсивно находим и добавляем в список подкатегории для категории subcategory 
            }
            return res; // метод возвращает список категорий
        }
    }
}
