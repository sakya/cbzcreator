using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Octokit;
using System.Threading.Tasks;
using Avalonia.SingleWindow.Abstracts;

namespace CbzCreatorGui.Utils
{
    public class AutoUpdater
    {
        private readonly GitHubClient? _client;
        public AutoUpdater()
        {
            _client = new GitHubClient(new ProductHeaderValue("CbzCreator"));
        }

        /// <summary>
        /// Check for an update (Windows only)
        /// </summary>
        /// <param name="owner">The window owner</param>
        /// <returns>True if an update has been found</returns>
        public async Task<bool> CheckForUpdate(Avalonia.Controls.Window owner)
        {
            IReadOnlyList<Release>? releases;
            try {
                releases = await _client!.Repository.Release.GetAll("sakya", "CbzCreator");
            } catch (Exception) {
                return false;
            }

            var release = releases?.Count > 0 ? releases[0] : null;
            if (release != null && release.Assets?.Count > 0) {
                if (Version.TryParse(GetVersionFromTagName(release.TagName), out var releaseVersion)) {
                    var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                    if (releaseVersion > currentVersion) {
                        // Merge changelogs
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(release.Body);
                        foreach (var r in releases!) {
                            if (r != release && Version.TryParse(GetVersionFromTagName(r.TagName), out releaseVersion)) {
                                if (releaseVersion > currentVersion) {
                                    sb.AppendLine(r.Body);
                                } else {
                                    break;
                                }
                            }
                        }

                        // Wait that other dialogs are closed
                        while (BaseDialog.CurrentDialog != null)
                            await Task.Delay(100);

                        var dlg = new Dialogs.UpdateDialog(release, sb.ToString());
                        await dlg.Show<bool?>();
                        return true;
                    }
                }
            }
            return false;
        } // CheckForUpdate

        private string GetVersionFromTagName(string tagname)
        {
            if (tagname.StartsWith("v"))
                tagname = tagname.Substring(1);
            return tagname;
        }
    }
}