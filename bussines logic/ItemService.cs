using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehousesystem.DAl;
using warehousesystem.models;

namespace warehousesystem.bussines_logic
{
    public class ItemService
    {
        private readonly ItemRepository _itemRepository;

        public ItemService()
        {
            _itemRepository = new ItemRepository();
        }

        public List<Item> GetAllItems()
        {
            return _itemRepository.GetAll();
        }

        public Item GetItemById(int id)
        {
            return _itemRepository.GetById(id);
        }

        public bool AddItem(Item item)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
            {
                throw new ArgumentException("اسم الصنف مطلوب.");
            }
            if (string.IsNullOrWhiteSpace(item.Code))
            {
                throw new ArgumentException("كود الصنف مطلوب.");
            }
            if (_itemRepository.GetItemByCode(item.Code) != null)
            {
                throw new InvalidOperationException($"الصنف بكود '{item.Code}' موجود بالفعل.");
            }

            _itemRepository.Add(item);
            return true;
        }

        public bool UpdateItem(Item item)
        {
            if (item.ItemID <= 0)
            {
                throw new ArgumentException("معرف الصنف غير صالح للتعديل.");
            }
            if (string.IsNullOrWhiteSpace(item.Name))
            {
                throw new ArgumentException("اسم الصنف مطلوب.");
            }
            if (string.IsNullOrWhiteSpace(item.Code))
            {
                throw new ArgumentException("كود الصنف مطلوب.");
            }

            var existingItem = _itemRepository.GetItemByCode(item.Code);
            if (existingItem != null && existingItem.ItemID != item.ItemID)
            {
                throw new InvalidOperationException($"الصنف بكود '{item.Code}' موجود بالفعل لصنف آخر.");
            }

            _itemRepository.Update(item);
            return true;
        }

        public void DeleteItem(int id)
        {
            _itemRepository.Delete(id);
        }
    }
}
