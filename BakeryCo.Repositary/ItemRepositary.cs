using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BakeryCo.DataModel;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace BakeryCo.Repositary
{
	public class ItemRepositary
	{
		BakeryCoEntities db = new BakeryCoEntities();

		//public ItemRepositary(BakeryCoEntities db)
		//{
		//    this.db = db;
		//}
		private bool disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					db.Dispose();
				}
			}
			this.disposed = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		public JObject GetMainCategories()
		{
			JObject Jobj = new JObject(new JProperty("Failure", "No Record Found"));
			try
			{
				// var CatM = db.MainCategories.Where(Cat => Cat.Status == true).ToList();
				var CatM = (from x in db.MainCategories
							where x.Status == true
							select (new { x.CategoryName, x.Description, x.CategoryName_Ar, x.CategoryId, x.Images, x.Description_Ar })).ToList();

				return CatM.Count > 0 ? Jobj = new JObject(new JProperty("Success", CatM)) : Jobj;
			}
			catch (Exception ex)
			{
				return Jobj = new JObject(new JProperty("Failure", ex.Message));

			}
		}

		public JObject GetSubCategoryDetails(int MainCatId)
		{
			JObject Jobj = new JObject(new JProperty("Failure", "No Record Found"));
			try
			{
				List<JArray> Jlist = new List<JArray>();
				var res = (from sCategory in db.SubCategories
						   join iDetails in db.ItemsDetails on sCategory.SubCategoryId equals iDetails.SubCatId
						   join iPrice in db.ItemPriceDetails on iDetails.ItemId equals iPrice.ItemId
						   where sCategory.CategoryId == MainCatId && sCategory.Status == true
						   select new
						   {
							   sCategory.CategoryId,
							   sName = sCategory.ItemName,
							   sName_Ar = sCategory.ItemName_Ar,
							   sDesc = sCategory.Description,
							   sDesc_Ar = sCategory.Description_Ar,
							   sCatId = sCategory.SubCategoryId,
							   iName = iDetails.ItemName,
							   iName_Ar = iDetails.ItemName_Ar,
							   iDesc = iDetails.ItemDesc,
							   iDesc_Ar = iDetails.ItemDesc_Ar,
							   iId = iDetails.ItemId,
							   iDetails.IsDeliver,
							   pId = iPrice.ItemId,
							   price = iPrice.ItemPrice,
							   size = iPrice.ItemSize,
							   Istatus = iDetails.Status,
							   pStatus = iPrice.Status,
							   iDetails.ItemSeq,
							   sCategory.SubCatSeq,
							   Simg = sCategory.Images,
							   Iimg = iDetails.images

						   }
						   ).OrderBy(o => o.sCatId).ToList();
				if (res.Count > 0)
				{
					int subCatId = 0;
					foreach (var sId in res.AsEnumerable().OrderBy(x => x.SubCatSeq))
					{
						List<JArray> JSubItems = new List<JArray>();
						if (subCatId != sId.sCatId)
						{
							subCatId = sId.sCatId;
							var subItems = res.Where(Id => Id.sCatId == sId.sCatId && Id.Istatus == true).OrderBy(itm => itm.ItemSeq);
							List<int> items = new List<int>();
							JArray jrSubItmes = new JArray(new JObject(new JProperty("sName", sId.sName),
															 new JProperty("sName_Ar", sId.sName_Ar),
															 new JProperty("sDesc", sId.sDesc),
															 new JProperty("sDesc_Ar", sId.sDesc_Ar),
															 new JProperty("Simg", sId.Simg),
															  new JProperty("MainCategory", sId.CategoryId),
															 new JProperty("subCatId", sId.sCatId)));
							foreach (var subItemDetails in subItems.AsEnumerable())
							{
								if (!items.Contains(subItemDetails.iId))
								{
									items.Add(subItemDetails.iId);
									var iDetails = res.Where(i => i.iId == subItemDetails.iId && i.Istatus == true).FirstOrDefault();//.OrderBy(id => id.ItemSeq)
									JArray jrItems = new JArray(new JObject(
																   new JProperty("iName", iDetails.iName),
																   new JProperty("iName_Ar", iDetails.iName_Ar),
																  new JProperty("iDesc", iDetails.iDesc),
																   new JProperty("iDesc_Ar", iDetails.iDesc_Ar),
																   new JProperty("Iimg", iDetails.Iimg),
																   new JProperty("IsDeliver", iDetails.IsDeliver),
																   new JProperty("ItemId", iDetails.iId)),
																   new JArray(from iPrice in res
																			  where iPrice.pId == subItemDetails.pId && iPrice.pStatus == true
																			  select new JObject(
																				  new JProperty("price", iPrice.price),
																				  new JProperty("size", iPrice.size),
																				  new JProperty("priceId", iPrice.pId))));
									JSubItems.Add(jrItems);
								}
							}
							jrSubItmes.Add(JSubItems);
							Jlist.Add(jrSubItmes);

						}

					}
				}
				Jobj = new JObject(new JProperty("Success", Jlist));
				return Jobj;
			}
			catch (Exception ex)
			{
				return Jobj = new JObject(new JProperty("Failure", ex.Message));
			}

		}
		public JObject GetSubCategoryDetailsOld()
		{
			JObject Jobj = new JObject(new JProperty("Failure", "No Record Found"));
			try
			{


				var res = db.SP_Get_ItemDetailsNew().ToList();

				if (res.Count > 0)
				{
					int CatId = 0;
					JArray jlaMainCategory = new JArray();
					foreach (var cId in res.AsEnumerable().OrderBy(x => x.CategoryId))
					{
						if (CatId != cId.CategoryId)
						{
							CatId = cId.CategoryId;

							int scatId = 0;
							JArray jlaSubCategory = new JArray();
							foreach (var sCat in res.AsEnumerable().OrderBy(x => x.SubCatSeq))
							{
								if (scatId != sCat.SubCategoryId && cId.CategoryId == sCat.CategoryId)
								{
									scatId = sCat.SubCategoryId;

									int secId = 0;
									JArray jlaSections = new JArray();
									foreach (var sec in res.AsEnumerable().OrderBy(x => x.SectionId))
									{
										if (secId != sec.SectionId)
										{
											secId = sec.SectionId;

											int itm = 0;
											JArray jlaItems = new JArray();
											foreach (var items in res.AsEnumerable().OrderBy(x => x.ItemSeq)) //Order by Seqence id 
											{
												if (itm != items.ItemId)
												{
													itm = items.ItemId;

													int? priceid = 0;
													JArray jlaprice = new JArray();
													foreach (var price in res.AsEnumerable().OrderBy(x => x.PriceId))
													{
														if (priceid != price.PriceId)
														{
															priceid = price.PriceId;
															if (price.ItempriceId == items.ItemId && items.ItemSectionId == sec.SectionId)
															{
																JObject japrice = new JObject(new JProperty("price", price.ItemPrice),
																								new JProperty("sizeId", price.ItemSize),
																								new JProperty("size", price.SizeName),
																								new JProperty("priceId", price.PriceId)
																			  );
																jlaprice.Add(japrice);
															}
														}
													}
													JArray jlaSubItems = new JArray();
													int? subitm = 0;
													foreach (var subitem in res.AsEnumerable().OrderBy(x => x.SubItemSeq))
													{
														if (subitm != subitem.SubItemsId)
														{
															//subitm = subitem.SubItemsId;
															if (subitem.SubItemsId == items.ItemId && items.ItemSectionId == sec.SectionId)
															{
																JObject jasubitem = new JObject(new JProperty("ItemId", subitem.SubItemsId),
																								new JProperty("SubItemId", subitem.SubItemsItemId),
																								new JProperty("SubItemName", subitem.SubItemName),
																								new JProperty("SubItemName_Ar", subitem.SubItemName_ar),
																								new JProperty("SubItemImage", subitem.SubItemImage),
																								new JProperty("SubItemSequence", subitem.SubItemSeq),
																								new JProperty("SubItemIsDeliver", subitem.SubItemIsDeliver));
																jlaSubItems.Add(jasubitem);
															}
														}
													}
													if (items.ItemSectionId == sec.SectionId && sCat.SubCategoryId == items.ItemSubCatId)
													{
														JObject jaitems = new JObject(new JProperty("ItemId", items.ItemId),
																				   new JProperty("SubCategoryId", items.ItemSubCatId),
																				   new JProperty("SectionId", items.SectionId),
																				   new JProperty("ItemName", items.ItemName),
																				   new JProperty("ItemName_Ar", items.ItemName_Ar),
																				   new JProperty("ItemDesc", items.ItemDesc),
																				   new JProperty("ItemDesc_Ar", items.ItemDesc_Ar),
																				   new JProperty("ItemImage", items.ItemImages),
																				   new JProperty("NoOfItems", items.NoOfItems),
																				   new JProperty("DisplayName", items.DisplayName),
																				   new JProperty("ItemSequence", items.ItemSeq),
																				   new JProperty("BindingType", items.BindingType),
																				   new JProperty("IsDeliver", items.IsDeliver),
																				   new JProperty("Price", jlaprice),
																				   new JProperty("SubItems", jlaSubItems));
														jlaItems.Add(jaitems);
													}
												}
											}
											JObject jasecCategory = new JObject(new JProperty("SectionId", sec.SectionId),
																   new JProperty("SectionName", sec.SectionName),
																   new JProperty("SectionSubCategory", sec.SectionSubCatId),
																	  new JProperty("Items", jlaItems)
																   );
											jlaSections.Add(jasecCategory);
										}
									}
									JObject jasubCategory = new JObject(new JProperty("SubCategoryId", sCat.SubCategoryId),
																	  new JProperty("SubCategoryName", sCat.SubCatgoryName),
																	  new JProperty("SubCategoryName_Ar", sCat.SubCategoryName_Ar),
																	  new JProperty("SubCategoryDesc", sCat.SubDescription),
																	  new JProperty("SubCategoryDesc_Ar", sCat.SubDescription_Ar),
																	  new JProperty("SubCategoryImage", sCat.SubImages),
																	  new JProperty("SubCategorySeq", sCat.SubCatSeq),
																	  new JProperty("MainCategoryId", sCat.SubMainCategoryId),
																	  new JProperty("Sections", jlaSections)
																	  );
									jlaSubCategory.Add(jasubCategory);
								}
							}
							//List<int> items = new List<int>();
							JObject mainCategory = new JObject(new JProperty("MainCategoryId", cId.CategoryId),
														 new JProperty("MainCategoryName", cId.CategoryName),
														 new JProperty("MainCategoryName_Ar", cId.CategoryName_Ar),
														 new JProperty("MainCategoryDesc", cId.MainDescription),
														 new JProperty("MainCategoryDesc_Ar", cId.MainDescription_Ar),
													 new JProperty("Simg", cId.MainImages),
													 new JProperty("SubCategoryItems", jlaSubCategory));
							jlaMainCategory.Add(mainCategory);
						}
					}

					Jobj = new JObject(new JProperty("Success", jlaMainCategory));

				}
				return Jobj;
			}
			catch (Exception ex)
			{
				return Jobj = new JObject(new JProperty("Failure", ex.Message));
			}

		}
		public JObject GetSubCategoryDetails()
		{
			JObject Jobj = new JObject(new JProperty("Failure", "No Record Found"));
			try
			{
				var res = db.SP_Get_ItemDetailsNew().ToList();

				var itmMainCat = (from x in res
								  select (new
								  {
									  x.CategoryId,
									  x.CategoryName,
									  x.CategoryName_Ar,
									  x.MainDescription,
									  x.MainDescription_Ar,
									  x.MainImages
								  })).Distinct().ToList().OrderBy(x => x.CategoryId);
				//list<int> catids
				//List<int> catIds = itmMainCat.Select(i => i.CategoryId).ToList();

				var itmSubCat = (from x in res
									 //where  catIds.Contains((int)x.SubMainCategoryId)
								 orderby x.SubCatSeq
								 select (new
								 {
									 x.SubCategoryId,
									 x.SubMainCategoryId,
									 x.SubCatgoryName,
									 x.SubCategoryName_Ar,
									 x.SubDescription,
									 x.SubDescription_Ar,
									 x.SubImages,
									 x.SubCatSeq
								 })).Distinct().ToList();
				//List<int> subCat = itmMainCat.Select(i => i.CategoryId).ToList();
				var itmSec = (from x in res
							  orderby x.SectionId
							  //where subCat.Contains((int)x.SubMainCategoryId)
							  select (new
							  {
								  x.SubCategoryId,
								  x.SubMainCategoryId,
								  x.SectionId,
								  x.SectionName
							  })).Distinct().ToList();
				var itmItems = (from x in res
								orderby x.ItemSeq
								//where catIds.Contains((int)x.)
								select (new
								{
									x.ItemId,
									x.ItemSubCatId,
									x.SubMainCategoryId,
									x.SectionId,
									x.ItemName,
									x.ItemName_Ar,
									x.ItemDesc,
									x.ItemDesc_Ar,
									x.ItemImages,
									x.ItemSeq,
									x.BindingType,
									x.IsDeliver,
									x.DisplayName,
									x.NoOfItems

								})).Distinct().ToList();
				var itmSubItems = (from x in res
									   //where catIds.Contains((int)x.SubMainCategoryId)
								   orderby x.SubItemSeq
								   select (new
								   {
									   x.SubItemsId,//ItemId
									   x.SubItemsItemId,
									   x.SubItemName,
									   x.SubItemName_ar,
									   x.SubItemDesc,
									   x.SubItemDesc_ar,
									   x.SubItemImage,
									   x.SubItemSeq,
									   x.SubItemIsDeliver
								   })).Distinct().ToList();
				var itmItemsPrice = (from x in res
										 //where catIds.Contains((int)x.SubMainCategoryId)
									 select (new
									 {
										 x.ItempriceId,
										 x.PriceId,
										 x.ItemPrice,
										 x.ItemSize,
										 x.ItemSizeDescription,
										 x.SizeName
									 })).Distinct().ToList().OrderBy(x => x.PriceId);
				JArray mainCatArry = new JArray();
				//main category loop
				foreach (var cId in itmMainCat)
				{
					var SubCat = (from sub in itmSubCat
								  where sub.SubMainCategoryId == cId.CategoryId
								  orderby sub.SubCatSeq
								  select
									 new
									 {
										 sub.SubCatgoryName,
										 sub.SubMainCategoryId,
										 sub.SubCategoryId,
										 sub.SubCategoryName_Ar,
										 sub.SubDescription,
										 sub.SubDescription_Ar,
										 sub.SubImages,
										 sub.SubCatSeq
									 }).Distinct().ToList();
					//sub category loop 
					JArray jlaSubCategory = new JArray();
					foreach (var sCat in SubCat)
					{
						var sect = (from sec in itmSec
									where sec.SubCategoryId == sCat.SubCategoryId
									//&& cId.CategoryId == sec.SubMainCategoryId
									select
										new { sec.SectionId, sec.SectionName, sec.SubCategoryId }).Distinct().ToList();
						//Section loop
						JArray jlaSections = new JArray();
						foreach (var sec in sect)
						{
							var items = (from itm in itmItems
										 where itm.SectionId == sec.SectionId
										 && itm.ItemSubCatId == sCat.SubCategoryId
										 && sCat.SubMainCategoryId == itm.SubMainCategoryId
										 orderby itm.ItemSeq
										 select
										 new
										 {
											 itm.ItemId,
											 itm.ItemSubCatId,
											 itm.SectionId,
											 itm.ItemName,
											 itm.ItemName_Ar,
											 itm.ItemDesc,
											 itm.ItemDesc_Ar,
											 itm.ItemImages,
											 itm.ItemSeq,
											 itm.BindingType,
											 itm.IsDeliver,
											 itm.DisplayName,
											 itm.NoOfItems
										 }).Distinct().ToList();
							// Items loop
							JArray jlaItems = new JArray();
							foreach (var item in items)
							{
								var subitems = (from sbitem in itmSubItems

												where sbitem.SubItemsId == item.ItemId && item.SectionId == sec.SectionId
												orderby sbitem.SubItemSeq

												select new
												{
													sbitem.SubItemsId,
													sbitem.SubItemsItemId,
													sbitem.SubItemName,
													sbitem.SubItemName_ar,
													sbitem.SubItemImage,
													sbitem.SubItemDesc,
													sbitem.SubItemDesc_ar,
													sbitem.SubItemSeq,
													sbitem.SubItemIsDeliver
												}).Distinct().ToList(); // need some other condition
								var itemprice = (from itmprice in itmItemsPrice
												 where itmprice.ItempriceId == item.ItemId
												 select new
												 {
													 itmprice.PriceId,
													 itmprice.ItemSize,
													 itmprice.SizeName,
													 itmprice.ItemPrice,
													 itmprice.ItemSizeDescription
												 }).Distinct().ToList();
								// SubItems Loop
								JArray jlaSubItems = new JArray();
								foreach (var sitem in subitems)
								{
									JObject jasubitem = new JObject(new JProperty("ItemId", sitem.SubItemsId),
																	new JProperty("SubItemId", sitem.SubItemsItemId),
																	new JProperty("SubItemName", sitem.SubItemName),
																	new JProperty("SubItemName_Ar", sitem.SubItemName_ar),
																	  new JProperty("SubItemDesc", sitem.SubItemDesc),
																	new JProperty("SubItemDesc_Ar", sitem.SubItemDesc_ar),
																	new JProperty("SubItemImage", sitem.SubItemImage),
																	new JProperty("SubItemSequence", sitem.SubItemSeq),
																	new JProperty("SubItemIsDeliver", sitem.SubItemIsDeliver));
									jlaSubItems.Add(jasubitem);
								}// end SubItems Loop
								 // ItemsPrice Loop
								JArray jlaprice = new JArray();
								foreach (var itmPrice in itemprice)
								{
									JObject japrice = new JObject(new JProperty("price", itmPrice.ItemPrice),
																  new JProperty("sizeId", itmPrice.ItemSize),
																  new JProperty("size", itmPrice.SizeName),
																  new JProperty("ItemSizeDescription", itmPrice.ItemSizeDescription),
																  new JProperty("priceId", itmPrice.PriceId)
															 );
									jlaprice.Add(japrice);
								}// end ItemsPrice loop
								JObject jaitems = new JObject(new JProperty("ItemId", item.ItemId),
													  new JProperty("SubCategoryId", item.ItemSubCatId),
													  new JProperty("SectionId", item.SectionId),
													  new JProperty("ItemName", item.ItemName),
													  new JProperty("ItemName_Ar", item.ItemName_Ar),
													  new JProperty("ItemDesc", item.ItemDesc),
													  new JProperty("ItemDesc_Ar", item.ItemDesc_Ar),
													  new JProperty("ItemImage", item.ItemImages),
													  new JProperty("ItemSequence", item.ItemSeq),
													  new JProperty("BindingType", item.BindingType),
													  new JProperty("IsDeliver", item.IsDeliver),
													  new JProperty("DisplayName", item.DisplayName),
													  new JProperty("NoOfItems", item.NoOfItems),
												  new JProperty("Price", jlaprice),
												  new JProperty("SubItems", jlaSubItems));
								jlaItems.Add(jaitems);
							} // end Items loop
							JObject jasecCategory = new JObject(new JProperty("SectionId", sec.SectionId),
												   new JProperty("SectionName", sec.SectionName),
												  new JProperty("SectionSubCategory", sec.SubCategoryId), // need to see
													 new JProperty("Items", jlaItems));
							jlaSections.Add(jasecCategory);
						}// end of Section loop
						JObject jasubCategory = new JObject(new JProperty("SubCategoryId", sCat.SubCategoryId),
															new JProperty("SubCategoryName", sCat.SubCatgoryName),
															new JProperty("SubCategoryName_Ar", sCat.SubCategoryName_Ar),
															new JProperty("SubCategoryDesc", sCat.SubDescription),
															new JProperty("SubCategoryDesc_Ar", sCat.SubDescription_Ar),
															new JProperty("SubCategoryImage", sCat.SubImages),
															new JProperty("SubCategorySeq", sCat.SubCatSeq),
															new JProperty("MainCategoryId", sCat.SubMainCategoryId),
															new JProperty("Sections", jlaSections));
						jlaSubCategory.Add(jasubCategory);
					}// end of category loop
					JObject mainCategory = new JObject(new JProperty("MainCategoryId", cId.CategoryId),
															 new JProperty("MainCategoryName", cId.CategoryName),
															 new JProperty("MainCategoryName_Ar", cId.CategoryName_Ar),
															 new JProperty("MainCategoryDesc", cId.MainDescription),
															 new JProperty("MainCategoryDesc_Ar", cId.MainDescription_Ar),
														 new JProperty("Simg", cId.MainImages),
														 new JProperty("SubCategoryItems", jlaSubCategory));
					mainCatArry.Add(mainCategory);
				}//End of main category loop

				Jobj = new JObject(new JProperty("Success", mainCatArry));
				return Jobj;
			}
			catch (Exception ex)
			{
				return Jobj = new JObject(new JProperty("Failure", ex.Message));
			}
		}

		public JObject GetSubCategoryItemsDetails()
		{
			JObject Jobj = new JObject(new JProperty("Failure", "No Record Found"));
			try
			{
				var res = db.SP_Get_ItemDetailsNew().ToList();

				var itmMainCat = (from x in res
								  select (new
								  {
									  x.CategoryId,
									  x.CategoryName,
									  x.CategoryName_Ar,
									  x.MainDescription,
									  x.MainDescription_Ar,
									  x.MainImages
								  })).Distinct().ToList().OrderBy(x => x.CategoryId);
				//list<int> catids
				//List<int> catIds = itmMainCat.Select(i => i.CategoryId).ToList();

				var itmSubCat = (from x in res
									 //where  catIds.Contains((int)x.SubMainCategoryId)
								 orderby x.SubCatSeq
								 select (new
								 {
									 x.SubCategoryId,
									 x.SubMainCategoryId,
									 x.SubCatgoryName,
									 x.SubCategoryName_Ar,
									 x.SubDescription,
									 x.SubDescription_Ar,
									 x.SubImages,
									 x.SubCatSeq
								 })).Distinct().ToList();
				//List<int> subCat = itmMainCat.Select(i => i.CategoryId).ToList();
				var itmSec = (from x in res
							  orderby x.SectionId
							  //where subCat.Contains((int)x.SubMainCategoryId)
							  select (new
							  {
								  x.SubCategoryId,
								  x.SubMainCategoryId,
								  x.SectionId,
								  x.SectionName,
								  x.SectionName_Ar
							  })).Distinct().ToList();
				var itmItems = (from x in res
								orderby x.ItemSeq
								//where catIds.Contains((int)x.)
								select (new
								{
									x.ItemId,
									x.ItemSubCatId,
									x.SubMainCategoryId,
									x.SectionId,
									x.ItemName,
									x.ItemName_Ar,
									x.ItemDesc,
									x.ItemDesc_Ar,
									x.ItemImages,
									x.ItemTypeIds,
									x.ItemSeq,
									x.BindingType,
									x.IsDeliver,
									x.DisplayName,
									x.DisplayName_Ar,
									x.NoOfItems

								})).Distinct().ToList();
				var itmSubItems = (from x in res
									   //where catIds.Contains((int)x.SubMainCategoryId)
								   orderby x.SubItemSeq
								   select (new
								   {
									   x.SubItemsId,//ItemId
									   x.SubItemsItemId,
									   x.SubItemName,
									   x.SubItemName_ar,
									   x.SubItemDesc,
									   x.SubItemDesc_ar,
									   x.SubItemImage,
									   x.SubItemSeq,
									   x.SubItemIsDeliver
								   })).Distinct().ToList();
				var itmItemsPrice = (from x in res
										 //where catIds.Contains((int)x.SubMainCategoryId)
									 select (new
									 {
										 x.ItempriceId,
										 x.PriceId,
										 x.ItemPrice,
										 x.ItemSize,
										 x.SizeName_Ar,
										 x.ItemSizeDescription,
										 x.ItemSizeDescription_Ar,
										 x.SizeName
									 })).Distinct().ToList().OrderBy(x => x.PriceId);
				JArray mainCatArry = new JArray();
				//main category loop
				foreach (var cId in itmMainCat)
				{
					var SubCat = (from sub in itmSubCat
								  where sub.SubMainCategoryId == cId.CategoryId
								  orderby sub.SubCatSeq
								  select
									 new
									 {
										 sub.SubCatgoryName,
										 sub.SubMainCategoryId,
										 sub.SubCategoryId,
										 sub.SubCategoryName_Ar,
										 sub.SubDescription,
										 sub.SubDescription_Ar,
										 sub.SubImages,
										 sub.SubCatSeq
									 }).Distinct().ToList();
					//sub category loop 
					JArray jlaSubCategory = new JArray();
					foreach (var sCat in SubCat)
					{
						var sect = (from sec in itmSec
									where sec.SubCategoryId == sCat.SubCategoryId
									//&& cId.CategoryId == sec.SubMainCategoryId
									select
										new { sec.SectionId, sec.SectionName, sec.SectionName_Ar, sec.SubCategoryId }).Distinct().ToList();
						//Section loop
						JArray jlaSections = new JArray();
						foreach (var sec in sect)
						{
							var items = (from itm in itmItems
										 where itm.SectionId == sec.SectionId
										 && itm.ItemSubCatId == sCat.SubCategoryId
										 && sCat.SubMainCategoryId == itm.SubMainCategoryId
										 orderby itm.ItemSeq
										 select
										 new
										 {
											 itm.ItemId,
											 itm.ItemSubCatId,
											 itm.SectionId,
											 itm.ItemName,
											 itm.ItemName_Ar,
											 itm.ItemDesc,
											 itm.ItemDesc_Ar,
											 itm.ItemImages,
											 itm.ItemSeq,
											 itm.ItemTypeIds,
											 itm.BindingType,
											 itm.IsDeliver,
											 itm.DisplayName_Ar,
											 itm.DisplayName,
											 itm.NoOfItems
										 }).Distinct().ToList();
							// Items loop
							JArray jlaItems = new JArray();
							foreach (var item in items)
							{
								var subitems = (from sbitem in itmSubItems

												where sbitem.SubItemsId == item.ItemId && item.SectionId == sec.SectionId
												orderby sbitem.SubItemSeq

												select new
												{
													sbitem.SubItemsId,
													sbitem.SubItemsItemId,
													sbitem.SubItemName,
													sbitem.SubItemName_ar,
													sbitem.SubItemImage,
													sbitem.SubItemDesc,
													sbitem.SubItemDesc_ar,
													sbitem.SubItemSeq,
													sbitem.SubItemIsDeliver
												}).Distinct().ToList(); // need some other condition
								var itemprice = (from itmprice in itmItemsPrice
												 where itmprice.ItempriceId == item.ItemId
												 select new
												 {
													 itmprice.PriceId,
													 itmprice.ItemSize,
													 itmprice.SizeName,
													 itmprice.SizeName_Ar,
													 itmprice.ItemPrice,
													 itmprice.ItemSizeDescription,
													 itmprice.ItemSizeDescription_Ar
												 }).Distinct().ToList();
								// SubItems Loop
								JArray jlaSubItems = new JArray();
								foreach (var sitem in subitems)
								{
									JObject jasubitem = new JObject(new JProperty("ItemId", sitem.SubItemsId),
																	new JProperty("SubItemId", sitem.SubItemsItemId),
																	new JProperty("SubItemName", sitem.SubItemName),
																	new JProperty("SubItemName_Ar", sitem.SubItemName_ar),
																	 new JProperty("SubItemDesc", sitem.SubItemDesc),
																	new JProperty("SubItemDesc_Ar", sitem.SubItemDesc_ar),
																	new JProperty("SubItemImage", sitem.SubItemImage),
																	new JProperty("SubItemSequence", sitem.SubItemSeq),
																	new JProperty("SubItemIsDeliver", sitem.SubItemIsDeliver));
									jlaSubItems.Add(jasubitem);
								}// end SubItems Loop
								 // ItemsPrice Loop
								JArray jlaprice = new JArray();
								foreach (var itmPrice in itemprice)
								{
									JObject japrice = new JObject(new JProperty("price", itmPrice.ItemPrice),
																  new JProperty("sizeId", itmPrice.ItemSize),
																  new JProperty("size", itmPrice.SizeName),
																  new JProperty("SizeName_Ar", itmPrice.SizeName_Ar),
																  new JProperty("ItemSizeDescription", itmPrice.ItemSizeDescription),
																  new JProperty("ItemSizeDescription_Ar", itmPrice.ItemSizeDescription_Ar),
																  new JProperty("priceId", itmPrice.PriceId)
															 );
									jlaprice.Add(japrice);
								}// end ItemsPrice loop
								JObject jaitems = new JObject(new JProperty("ItemId", item.ItemId),
													  new JProperty("SubCategoryId", item.ItemSubCatId),
													  new JProperty("SectionId", item.SectionId),
													  new JProperty("ItemName", item.ItemName),
													  new JProperty("ItemName_Ar", item.ItemName_Ar),
													  new JProperty("ItemDesc", item.ItemDesc),
													  new JProperty("ItemDesc_Ar", item.ItemDesc_Ar),
													  new JProperty("ItemImage", item.ItemImages),
													   new JProperty("ItemTypeIds", item.ItemTypeIds),
													  new JProperty("ItemSequence", item.ItemSeq),
													  new JProperty("BindingType", item.BindingType),
													  new JProperty("IsDeliver", item.IsDeliver),
													  new JProperty("DisplayName", item.DisplayName),
													  new JProperty("DisplayName_Ar", item.DisplayName_Ar),
													  new JProperty("NoOfItems", item.NoOfItems),
												  new JProperty("Price", jlaprice),
												  new JProperty("SubItems", jlaSubItems));
								jlaItems.Add(jaitems);
							} // end Items loop
							JObject jasecCategory = new JObject(new JProperty("SectionId", sec.SectionId),
												   new JProperty("SectionName", sec.SectionName),
													new JProperty("SectionName_Ar", sec.SectionName_Ar),
												  new JProperty("SectionSubCategory", sec.SubCategoryId), // need to see
													 new JProperty("Items", jlaItems));
							jlaSections.Add(jasecCategory);
						}// end of Section loop
						JObject jasubCategory = new JObject(new JProperty("SubCategoryId", sCat.SubCategoryId),
															new JProperty("SubCategoryName", sCat.SubCatgoryName),
															new JProperty("SubCategoryName_Ar", sCat.SubCategoryName_Ar),
															new JProperty("SubCategoryDesc", sCat.SubDescription),
															new JProperty("SubCategoryDesc_Ar", sCat.SubDescription_Ar),
															new JProperty("SubCategoryImage", sCat.SubImages),
															new JProperty("SubCategorySeq", sCat.SubCatSeq),
															new JProperty("MainCategoryId", sCat.SubMainCategoryId),
															new JProperty("Sections", jlaSections));
						jlaSubCategory.Add(jasubCategory);
					}// end of category loop
					JObject mainCategory = new JObject(new JProperty("MainCategoryId", cId.CategoryId),
															 new JProperty("MainCategoryName", cId.CategoryName),
															 new JProperty("MainCategoryName_Ar", cId.CategoryName_Ar),
															 new JProperty("MainCategoryDesc", cId.MainDescription),
															 new JProperty("MainCategoryDesc_Ar", cId.MainDescription_Ar),
														 new JProperty("Simg", cId.MainImages),
														 new JProperty("SubCategoryItems", jlaSubCategory));
					mainCatArry.Add(mainCategory);
				}//End of main category loop

				Jobj = new JObject(new JProperty("Success", mainCatArry));
				return Jobj;
			}
			catch (Exception ex)
			{
				return Jobj = new JObject(new JProperty("Failure", ex.Message));
			}
		}

		public JObject GetSubCategoryItemsDetailsNew()
		{
			JObject Jobj = new JObject(new JProperty("Failure", "No Record Found"));
			try
			{
				JArray jarr = new JArray();
				JArray jbanner = new JArray();
				var data = DbConnector.ExecuteReader("SP_Get_ItemDetailsNew1", null);
				if (data != null && data.HasRows)
				{
					while (data.Read())
					{
						jarr = JArray.Parse(data["Result"].ToString());
					}
					data.Close();
				}
				//var res = db.SP_Get_ItemDetailsNewTest();

				//jarr = JArray.Parse(res.ToString());

				Jobj = new JObject(new JProperty("Success", jarr));
				return Jobj;
			}
			catch (Exception ex)
			{
				return Jobj = new JObject(new JProperty("Failure", ex.Message));
			}
		}
		public JObject GetSubCategoryItemsDetailsWithBanners()
		{
			JObject Jobj = new JObject(new JProperty("Failure", "No Record Found"));
			try
			{
				JArray jarr = new JArray();
				JArray jbanner = new JArray();
				JArray jbPaymentMode = new JArray();
				var data = DbConnector.ExecuteReader("SP_Get_ItemDetailsNew1", null);
				if (data != null && data.HasRows)
				{
					while (data.Read())
					{
						jarr = JArray.Parse(data["Result"].ToString());
						jbanner = JArray.Parse(data["Banners"].ToString());
						jbPaymentMode = JArray.Parse(data["PaymentMode"].ToString());
					}
					data.Close();
				}
				//var res = db.SP_Get_ItemDetailsNewTest();
				//jarr = JArray.Parse(res.ToString());

				JObject item = new JObject(new JProperty("Items", jarr),
											new JProperty("Banners", jbanner),
											new JProperty("PaymentMode", jbPaymentMode));

				Jobj = new JObject(new JProperty("Success", item));

				return Jobj;
			}
			catch (Exception ex)
			{
				return Jobj = new JObject(new JProperty("Failure", ex.Message));
			}
		}

		public JObject GetPromotionsList()
		{
			JObject Jobj = new JObject(new JProperty("Failure", "No Record Found"));
			try
			{
				JArray jarr = new JArray();
				JArray jbanner = new JArray();
				JArray jbPaymentMode = new JArray();
				var data = DbConnector.ExecuteReader("[dbo].[SP_Get_PromotionList]", null);
				if (data != null && data.HasRows)
				{
					while (data.Read())
					{
						jarr = JArray.Parse(data["PromotionList"].ToString());
					}
					data.Close();
				}
				 
				JObject item = new JObject(new JProperty("PromotionList", jarr));

				Jobj = new JObject(new JProperty("Success", item));

				return Jobj;
			}
			catch (Exception ex)
			{
				return Jobj = new JObject(new JProperty("Failure", ex.Message));
			}
		}

	}
}
