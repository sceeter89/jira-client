# JIRA Assistant

This application's aim is to make interaction with JIRA more pleasant and powerful. At the same time it serves as a programming playground for me, to try out new patterns, new stuff and to learn. I hope to create something useful at the same time learning as much as I can. Currently supported features are:
* Browsing JIRA Agile boards and sprints (in case of Scrum Board)
* Search for issues and save filters
* Pivot reporting
* Print Scrum cards either for sprint or for any set of issues with customizable card template (includes ability to place color tags on any card before printing)
* Analysis of open Epics
* Inspection of Scrum Team velocity and capacity
* "_Issues Graveyard_" - hints on which tasks that should probably be already closed
* Export list of issues to text or Confluence Markup
* Auto-updater

## How to install

Please download latest release from: https://github.com/sceeter89/jira-client/releases and run installer. It's signed with my public key available via [Keybase.io](https://keybase.io) profile.

### Prerequisities

To run _JIRA Assistant_ you will need to install .NET 4.5.2 (https://www.microsoft.com/pl-pl/download/details.aspx?id=42642) or higher.

## Feedback

Any feedback is greatly welcome. Whether you think of some cool feature to add or you encounter some bug or trouble feel free to raise an issue here: https://github.com/sceeter89/jira-manager/issues

## Development

_JIRA Assistant_ is being developed using Visual Studio 2015 Community edition and installer is created by Advanced Installer. Application makes great use of Telerik controls for WPF, so to build it you will need these controls installed (http://www.telerik.com/products/wpf/overview.aspx).
