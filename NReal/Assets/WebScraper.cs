// using System.Collections;
// using System.Collections.Generic;
// using System.Net;
// using HtmlAgilityPack;
// using UnityEngine;
//
// public class GoogleScraper
// {
//     public string GetLocationInfo(string query) {
//         string url = "https://www.google.com/search?q=" + query;
//         string htmlContent = "";
//
//         // Download the HTML content of the search results page
//         using (WebClient client = new WebClient()) {
//             htmlContent = client.DownloadString(url);
//         }
//
//         // Parse the HTML content using HtmlAgilityPack
//         HtmlDocument doc = new HtmlDocument();
//         doc.LoadHtml(htmlContent);
//
//         // Get the location name, description, review, and address
//         string location = doc.DocumentNode.SelectSingleNode("//div[@class='SPZz6b']/h1/span[contains(@class,'SPZz6b')]").InnerText;
//         string description = doc.DocumentNode.SelectSingleNode("//span[contains(@class,'Y3v8qd')]").InnerText;
//         string review = doc.DocumentNode.SelectSingleNode("//span[contains(@class,'Aq14fc')]/span[contains(@class,'SJYcg')]/span[contains(@class,'fYOrjf')]/span[contains(@class,'Jtu6Td')]/span[contains(@class,'OWXK8c')]/span[contains(@class,'')]").InnerText;
//         string address = doc.DocumentNode.SelectSingleNode("//div[contains(@class,'BNeawe iBp4i AP7Wnd') and contains(text(),'Address')]/following-sibling::div[1]").InnerText;
//
//         // Return the location info as a string
//         return location + "\n" + description + "\n" + review + "\n" + address;
//     }
// }
