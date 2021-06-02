using System;
using System.Collections.Generic;
using AnomalyService.Models;

namespace AnomalyService.Helpers
{
    public class ImageWithUrlHelper
    {
        public ImageWithUrlHelper()
        {
            
        }

        public Dictionary<string, string> WithUrl(Image image)
        {
            Dictionary<string, string> imageDict = new Dictionary<string, string>();
            imageDict.Add("id", image.Id.ToString());
            imageDict.Add("name", image.Name.ToString());
            imageDict.Add("road", image.Road.ToString());
            imageDict.Add("kp", image.Kp.ToString());
            imageDict.Add("latitude", image.Latitude.ToString());
            imageDict.Add("longitude", image.Longitude.ToString());
            imageDict.Add("takenAt", image.TakenAt.ToString());
            imageDict.Add("createdAt", image.CreatedAt.ToString());
            imageDict.Add("updatedAt", image.UpdatedAt.ToString());
            imageDict.Add("url", new AzureStorageHelper().GetServiceSasUriForBlob(image.Name).ToString());
            return imageDict;
        }

        public List< Dictionary<string, string> > ToList(List<Image> images)
        {
            List<Dictionary<string, string>> imageList = new List<Dictionary<string, string>>();

            foreach (Image image in images) {
                imageList.Add(WithUrl(image));
            }
            
            return imageList;

        }
    }
}
