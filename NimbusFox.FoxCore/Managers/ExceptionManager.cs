using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Windows.Forms;
using NimbusFox.FoxCore.Classes;
using NimbusFox.FoxCore.Dependencies.Newtonsoft.Json;
using Plukit.Base;

namespace NimbusFox.FoxCore.Managers {
    public class ExceptionManager {
        private string _modVersion;
        private readonly DirectoryManager _errorsDir;
        private readonly string _errorEmail;
        private readonly string _author;
        private readonly string _mod;

        internal ExceptionManager(string author, string mod, string modVersion, string errorEmail = null) {
            _errorsDir = new DirectoryManager().FetchDirectoryNoParent("modErrors").FetchDirectoryNoParent(mod);
            _modVersion = modVersion;
            _errorEmail = errorEmail;
            _author = author;
            _mod = mod;
        }

        public void HandleException(Exception ex, Dictionary<string, object> extras = null) {
            var filename = DateTime.Now.Ticks;

            _errorsDir.WriteFile($"{filename}.{_modVersion}.error", JsonConvert.SerializeObject(ex, Formatting.Indented), null, true);

            if (extras != null) {
                if (extras.Any()) {
                    _errorsDir.WriteFile($"{filename}.{_modVersion}.data", JsonConvert.SerializeObject(extras, Formatting.Indented), null, true);
                }
            }

            if (_errorEmail != null) {
                if (CoreHook.FxCore.ConfigDirectory.FetchDirectory("errorReports")
                    .FileExists($"{_author}.{_mod}.{_modVersion}.config")) {
                    var blob = CoreHook.FxCore.ConfigDirectory.FetchDirectory("errorReports")
                        .ReadFile<Blob>($"{_author}.{_mod}.{_modVersion}.config", true);

                    if (blob.GetBool("report", false)) {
                        var smtpClient = new SmtpClient("srv73.hosting24.com") {
                            Timeout = 10000,
                            EnableSsl = true,
                            Credentials = new NetworkCredential("foxcore@nimbusfox.uk", "_tn{f!G{Dp8v")
                        };

                        var mail = new MailMessage();
                        try {
                            mail.From = new MailAddress("foxcore@nimbusfox.uk");

                            mail.To.Add(_errorEmail);

                            mail.Subject = ex.Message;

                            mail.Body = "Exception:" + Environment.NewLine + JsonConvert.SerializeObject(ex) + Environment.NewLine +
                                        Environment.NewLine;

                            if (extras != null) {
                                mail.Body += "Data:" + Environment.NewLine + JsonConvert.SerializeObject(extras);
                            }

                            smtpClient.Send(mail);
                        } catch {
                            smtpClient.Dispose();

                            mail.Dispose();
                            // if mail fails to send do nothing
                        }
                    }
                }
            }
        }
    }
}
