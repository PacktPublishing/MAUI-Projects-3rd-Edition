namespace GalleryApp.Services;

using GalleryApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Microsoft.Search.Interop;
using System.Data.OleDb;

internal partial class PhotoImporter
{
	ISearchQueryHelper queryHelper;

	private partial async Task<string[]> Import()
	{
		var paths = new List<string>();

		var status = await AppPermissions.CheckAndRequestRequiredPermission();
		if (status == PermissionStatus.Granted)
		{
			string sqlQuery = queryHelper.GenerateSQLFromUserQuery(" ");

			using OleDbConnection conn = new(queryHelper.ConnectionString);
			conn.Open();

			using OleDbCommand command = new(sqlQuery, conn);
			using OleDbDataReader WDSResults = command.ExecuteReader();

			while (WDSResults.Read())
			{
				var itemUrl = WDSResults.GetString(0);
				paths.Add(itemUrl);
			}
		}
		return paths.ToArray();
	}

	public partial async Task<ObservableCollection<Photo>> Get(int start, int count, Quality quality)
	{
		string[] patterns = { ".png", ".jpeg", ".jpg" };

		string[] locations = {
			Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
			Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures),
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),"OneDrive","Camera Roll")
		};

		queryHelper = new CSearchManager().GetCatalog("SystemIndex").GetQueryHelper();

		queryHelper.QueryMaxResults = start + count;

		queryHelper.QuerySelectColumns = "System.ItemUrl";

		queryHelper.QueryWhereRestrictions = "AND (";
		foreach (var pattern in patterns)
			queryHelper.QueryWhereRestrictions += " Contains(System.FileExtension, '" + pattern + "') OR";
		queryHelper.QueryWhereRestrictions = queryHelper.QueryWhereRestrictions[..^2];
		queryHelper.QueryWhereRestrictions += ")";

		queryHelper.QueryWhereRestrictions += " AND (";
		foreach (var location in locations)
			queryHelper.QueryWhereRestrictions += " scope='" + location + "' OR";
		queryHelper.QueryWhereRestrictions = queryHelper.QueryWhereRestrictions[..^2];
		queryHelper.QueryWhereRestrictions += ")";

		queryHelper.QuerySorting = "System.DateModified DESC";

		var photos = new ObservableCollection<Photo>();

		var result = await Import();
		if (result?.Length == 0)
		{
			return photos;
		}

		Index startIndex = start;
		Index endIndex = start + count;

		if (endIndex.Value >= result.Length)
		{
			endIndex = result.Length;
		}

		if (startIndex.Value > endIndex.Value)
		{
			return photos;
		}

		foreach (var uri in result[startIndex..endIndex])
		{
			var path = new Uri(uri).AbsolutePath;
            photos.Add(new()
            {
                Bytes = File.ReadAllBytes(path),
                Filename = Path.GetFileName(path)
            });
        }
        return photos;
	}

	public partial async Task<ObservableCollection<Photo>> Get(List<string> filenames, Quality quality)
	{
		queryHelper = new CSearchManager().GetCatalog("SystemIndex").GetQueryHelper();

        queryHelper.QuerySelectColumns = "System.ItemUrl";

        queryHelper.QueryWhereRestrictions = "AND (";
        foreach (var filename in filenames)
			queryHelper.QueryWhereRestrictions += " Contains(System.Filename, '" + filename + "') OR";
		queryHelper.QueryWhereRestrictions = queryHelper.QueryWhereRestrictions[..^2];
        queryHelper.QueryWhereRestrictions += ")";

        var photos = new ObservableCollection<Photo>();

        var result = await Import();
        if (result?.Length == 0)
        {
            return photos;
        }

        foreach (var uri in result)
        {
            var path = new Uri(uri).AbsolutePath;
			var filename = Path.GetFileName(path);
            if (filenames.Contains(filename))
			{
                photos.Add(new()
                {
                    Bytes = File.ReadAllBytes(path),
                    Filename = filename
                });
            }
        }
        return photos;
    }
}
