using System;
using System.Linq;
using System.IO;
using data;
using model;
using api.Dtos;
using System.Collections.Generic;
using framework.Extensions;
using framework;

namespace api
{
    public class FileManager
    {
        private Repository<Arquivo> ArquivoRepository;
        private static Dictionary<string, string> _tipos = null;

        public FileManager(Repository<Arquivo> arquivoRepository)
        {
            ArquivoRepository = arquivoRepository;
        }

        public virtual Arquivo SaveFile(Arquivo arquivo, string base64)
        {
            ArquivoRepository.Insert(arquivo);

            var fileName = GetFilePath(arquivo);
            var index = base64.Substring(0, 200).IndexOf(",");
            var startIndex = 0;
            if (index > -1)
            {
                startIndex = index + 1;
            }
            var length = base64.Length - startIndex;

            System.IO.File.WriteAllBytes(
                fileName,
                Convert.FromBase64String(base64.Substring(startIndex, length))
            );

            ArquivoRepository.Update(arquivo);

            return arquivo;
        }

        public virtual Stream Get(string fileName)
        {
            var id = long.Parse(Path.GetFileNameWithoutExtension(fileName));
            var arquivo = ArquivoRepository.Get(id);
            return Get(arquivo);
        }

        public virtual byte[] GetBytes(string fileName)
        {
            var id = long.Parse(Path.GetFileNameWithoutExtension(fileName));
            var arquivo = ArquivoRepository.Get(id);
            return GetBytes(arquivo);
        }

        public virtual byte[] GetBytes(Arquivo arquivo)
        {
            var caminhoDoArquivo = GetFilePath(arquivo);
            var nomeDoArquivo = string.Format("{0}.{1}", arquivo.Id, GetExtension(arquivo.Tipo));
            var bytes = System.IO.File.ReadAllBytes(caminhoDoArquivo);
            return bytes;
        }

        public virtual Stream Get(Arquivo arquivo)
        {
            var caminhoDoArquivo = GetFilePath(arquivo);
            var nomeDoArquivo = string.Format("{0}.{1}", arquivo.Id, GetExtension(arquivo.Tipo));
            var stream = new FileStream(caminhoDoArquivo, FileMode.Open, FileAccess.Read);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public virtual ArquivoDto GetInfo(string fileName)
        {
            var id = long.Parse(Path.GetFileNameWithoutExtension(fileName));
            var arquivo = ArquivoRepository.Get(id);
            var result = ArquivoDto.Build(arquivo);
            return result;
        }

        public virtual Stream DownloadPublicFile(string fileName)
        {
            var caminhoDoArquivo = default(string);
            var stream = default(Stream);
            // Qualquer arquivo fixo que estiver na pasta /content/docs
            var ARQUIVOS_PUBLICOS = new string[] {
                "MODELO-IMPORTACAO-EQUIPAMENTO.xlsx"
            };
            if (ARQUIVOS_PUBLICOS.Contains(fileName))
            {
                caminhoDoArquivo = Path.Combine(AppContext.BaseDirectory, $"content/docs/{fileName}");
            }
            if (!string.IsNullOrEmpty(caminhoDoArquivo))
            {
                stream = new FileStream(caminhoDoArquivo, FileMode.Open);
                stream.Seek(0, SeekOrigin.Begin);
            }
            return stream;
        }

        public virtual string GetFilePath(Arquivo arquivo)
        {
            if (string.IsNullOrWhiteSpace(arquivo.Nome) || arquivo.Nome.EndsWith(".unknown"))
            {
                arquivo.Nome = string.Format("{0}.{1}", arquivo.Id, GetExtension(arquivo.Tipo));
            }
            var fileName = Path.Combine(Cfg.FileStoragePath, arquivo.Nome);
            return fileName;
        }

        public virtual string GetFilePath(string fileName)
        {
            var result = Path.Combine(Cfg.FileStoragePath, fileName);
            return result;
        }

        public static string GetExtension(string tipo)
        {
            if (_tipos == null)
            {
                #region Tipos
                _tipos = new Dictionary<string, string>();
                _tipos.Add("application/vnd.hzn-3d-crossword", "x3d");
                _tipos.Add("video/3gpp", "3gp");
                _tipos.Add("video/3gpp2", "3g2");
                _tipos.Add("application/vnd.mseq", "mseq");
                _tipos.Add("application/vnd.3m.post-it-notes", "pwn");
                _tipos.Add("application/vnd.3gpp.pic-bw-large", "plb");
                _tipos.Add("application/vnd.3gpp.pic-bw-small", "psb");
                _tipos.Add("application/vnd.3gpp.pic-bw-var", "pvb");
                _tipos.Add("application/vnd.3gpp2.tcap", "tcap");
                _tipos.Add("application/x-7z-compressed", "7z");
                _tipos.Add("application/x-abiword", "abw");
                _tipos.Add("application/x-ace-compressed", "ace");
                _tipos.Add("application/vnd.americandynamics.acc", "acc");
                _tipos.Add("application/vnd.acucobol", "acu");
                _tipos.Add("application/vnd.acucorp", "atc");
                _tipos.Add("audio/adpcm", "adp");
                _tipos.Add("application/x-authorware-bin", "aab");
                _tipos.Add("application/x-authorware-map", "aam");
                _tipos.Add("application/x-authorware-seg", "aas");
                _tipos.Add("application/vnd.adobe.air-application-installer-package+zip", "air");
                _tipos.Add("application/x-shockwave-flash", "swf");
                _tipos.Add("application/vnd.adobe.fxp", "fxp");
                _tipos.Add("application/pdf", "pdf");
                _tipos.Add("application/vnd.cups-ppd", "ppd");
                _tipos.Add("application/x-director", "dir");
                _tipos.Add("application/vnd.adobe.xdp+xml", "xdp");
                _tipos.Add("application/vnd.adobe.xfdf", "xfdf");
                _tipos.Add("audio/x-aac", "aac");
                _tipos.Add("application/vnd.ahead.space", "ahead");
                _tipos.Add("application/vnd.airzip.filesecure.azf", "azf");
                _tipos.Add("application/vnd.airzip.filesecure.azs", "azs");
                _tipos.Add("application/vnd.amazon.ebook", "azw");
                _tipos.Add("application/vnd.amiga.ami", "ami");
                _tipos.Add("application/andrew-inset", "N/A");
                _tipos.Add("application/vnd.android.package-archive", "apk");
                _tipos.Add("application/vnd.anser-web-certificate-issue-initiation", "cii");
                _tipos.Add("application/vnd.anser-web-funds-transfer-initiation", "fti");
                _tipos.Add("application/vnd.antix.game-component", "atx");
                _tipos.Add("application/x-apple-diskimage", "dmg");
                _tipos.Add("application/vnd.apple.installer+xml", "mpkg");
                _tipos.Add("application/applixware", "aw");
                _tipos.Add("audio/mpeg", "mp3");
                _tipos.Add("application/vnd.hhe.lesson-player", "les");
                _tipos.Add("application/vnd.aristanetworks.swi", "swi");
                _tipos.Add("text/x-asm", "s");
                _tipos.Add("application/atomcat+xml", "atomcat");
                _tipos.Add("application/atomsvc+xml", "atomsvc");
                _tipos.Add("application/atom+xml", "atom");
                _tipos.Add("application/pkix-attr-cert", "ac");
                _tipos.Add("audio/x-aiff", "aif");
                _tipos.Add("video/x-msvideo", "avi");
                _tipos.Add("application/vnd.audiograph", "aep");
                _tipos.Add("image/vnd.dxf", "dxf");
                _tipos.Add("model/vnd.dwf", "dwf");
                _tipos.Add("text/plain-bas", "par");
                _tipos.Add("application/x-bcpio", "bcpio");
                _tipos.Add("application/octet-stream", "bin");
                _tipos.Add("image/bmp", "bmp");
                _tipos.Add("image/jpg", "jpg");
                _tipos.Add("image/jpeg", "jpeg");
                _tipos.Add("image/png", "png");
                _tipos.Add("application/x-bittorrent", "torrent");
                _tipos.Add("application/vnd.rim.cod", "cod");
                _tipos.Add("application/vnd.blueice.multipass", "mpm");
                _tipos.Add("application/vnd.bmi", "bmi");
                _tipos.Add("application/x-sh", "sh");
                _tipos.Add("image/prs.btif", "btif");
                _tipos.Add("application/vnd.businessobjects", "rep");
                _tipos.Add("application/x-bzip", "bz");
                _tipos.Add("application/x-bzip2", "bz2");
                _tipos.Add("application/x-csh", "csh");
                _tipos.Add("text/x-c", "c");
                _tipos.Add("application/vnd.chemdraw+xml", "cdxml");
                _tipos.Add("text/css", "css");
                _tipos.Add("chemical/x-cdx", "cdx");
                _tipos.Add("chemical/x-cml", "cml");
                _tipos.Add("chemical/x-csml", "csml");
                _tipos.Add("application/vnd.contact.cmsg", "cdbcmsg");
                _tipos.Add("application/vnd.claymore", "cla");
                _tipos.Add("application/vnd.clonk.c4group", "c4g");
                _tipos.Add("image/vnd.dvb.subtitle", "sub");
                _tipos.Add("application/cdmi-capability", "cdmia");
                _tipos.Add("application/cdmi-container", "cdmic");
                _tipos.Add("application/cdmi-domain", "cdmid");
                _tipos.Add("application/cdmi-object", "cdmio");
                _tipos.Add("application/cdmi-queue", "cdmiq");
                _tipos.Add("application/vnd.cluetrust.cartomobile-config", "c11amc");
                _tipos.Add("application/vnd.cluetrust.cartomobile-config-pkg", "c11amz");
                _tipos.Add("image/x-cmu-raster", "ras");
                _tipos.Add("model/vnd.collada+xml", "dae");
                _tipos.Add("text/csv", "csv");
                _tipos.Add("application/mac-compactpro", "cpt");
                _tipos.Add("application/vnd.wap.wmlc", "wmlc");
                _tipos.Add("image/cgm", "cgm");
                _tipos.Add("x-conference/x-cooltalk", "ice");
                _tipos.Add("image/x-cmx", "cmx");
                _tipos.Add("application/vnd.xara", "xar");
                _tipos.Add("application/vnd.cosmocaller", "cmc");
                _tipos.Add("application/x-cpio", "cpio");
                _tipos.Add("application/vnd.crick.clicker", "clkx");
                _tipos.Add("application/vnd.crick.clicker.keyboard", "clkk");
                _tipos.Add("application/vnd.crick.clicker.palette", "clkp");
                _tipos.Add("application/vnd.crick.clicker.template", "clkt");
                _tipos.Add("application/vnd.crick.clicker.wordbank", "clkw");
                _tipos.Add("application/vnd.criticaltools.wbs+xml", "wbs");
                _tipos.Add("application/vnd.rig.cryptonote", "cryptonote");
                _tipos.Add("chemical/x-cif", "cif");
                _tipos.Add("chemical/x-cmdf", "cmdf");
                _tipos.Add("application/cu-seeme", "cu");
                _tipos.Add("application/prs.cww", "cww");
                _tipos.Add("text/vnd.curl", "curl");
                _tipos.Add("text/vnd.curl.dcurl", "dcurl");
                _tipos.Add("text/vnd.curl.mcurl", "mcurl");
                _tipos.Add("text/vnd.curl.scurl", "scurl");
                _tipos.Add("application/vnd.curl.car", "car");
                _tipos.Add("application/vnd.curl.pcurl", "pcurl");
                _tipos.Add("application/vnd.yellowriver-custom-menu", "cmp");
                _tipos.Add("application/dssc+der", "dssc");
                _tipos.Add("application/dssc+xml", "xdssc");
                _tipos.Add("application/x-debian-package", "deb");
                _tipos.Add("audio/vnd.dece.audio", "uva");
                _tipos.Add("image/vnd.dece.graphic", "uvi");
                _tipos.Add("video/vnd.dece.hd", "uvh");
                _tipos.Add("video/vnd.dece.mobile", "uvm");
                _tipos.Add("video/vnd.uvvu.mp4", "uvu");
                _tipos.Add("video/vnd.dece.pd", "uvp");
                _tipos.Add("video/vnd.dece.sd", "uvs");
                _tipos.Add("video/vnd.dece.video", "uvv");
                _tipos.Add("application/x-dvi", "dvi");
                _tipos.Add("application/vnd.fdsn.seed", "seed");
                _tipos.Add("application/x-dtbook+xml", "dtb");
                _tipos.Add("application/x-dtbresource+xml", "res");
                _tipos.Add("application/vnd.dvb.ait", "ait");
                _tipos.Add("application/vnd.dvb.service", "svc");
                _tipos.Add("audio/vnd.digital-winds", "eol");
                _tipos.Add("image/vnd.djvu", "djvu");
                _tipos.Add("application/xml-dtd", "dtd");
                _tipos.Add("application/vnd.dolby.mlp", "mlp");
                _tipos.Add("application/x-doom", "wad");
                _tipos.Add("application/vnd.dpgraph", "dpg");
                _tipos.Add("audio/vnd.dra", "dra");
                _tipos.Add("application/vnd.dreamfactory", "dfac");
                _tipos.Add("audio/vnd.dts", "dts");
                _tipos.Add("audio/vnd.dts.hd", "dtshd");
                _tipos.Add("image/vnd.dwg", "dwg");
                _tipos.Add("application/vnd.dynageo", "geo");
                _tipos.Add("application/ecmascript", "es");
                _tipos.Add("application/vnd.ecowin.chart", "mag");
                _tipos.Add("image/vnd.fujixerox.edmics-mmr", "mmr");
                _tipos.Add("image/vnd.fujixerox.edmics-rlc", "rlc");
                _tipos.Add("application/exi", "exi");
                _tipos.Add("application/vnd.proteus.magazine", "mgz");
                _tipos.Add("application/epub+zip", "epub");
                _tipos.Add("message/rfc822", "eml");
                _tipos.Add("application/vnd.enliven", "nml");
                _tipos.Add("application/vnd.is-xpr", "xpr");
                _tipos.Add("image/vnd.xiff", "xif");
                _tipos.Add("application/vnd.xfdl", "xfdl");
                _tipos.Add("application/emma+xml", "emma");
                _tipos.Add("application/vnd.ezpix-album", "ez2");
                _tipos.Add("application/vnd.ezpix-package", "ez3");
                _tipos.Add("image/vnd.fst", "fst");
                _tipos.Add("video/vnd.fvt", "fvt");
                _tipos.Add("image/vnd.fastbidsheet", "fbs");
                _tipos.Add("application/vnd.denovo.fcselayout-link", "fe_launch");
                _tipos.Add("video/x-f4v", "f4v");
                _tipos.Add("video/x-flv", "flv");
                _tipos.Add("image/vnd.fpx", "fpx");
                _tipos.Add("image/vnd.net-fpx", "npx");
                _tipos.Add("text/vnd.fmi.flexstor", "flx");
                _tipos.Add("video/x-fli", "fli");
                _tipos.Add("application/vnd.fluxtime.clip", "ftc");
                _tipos.Add("application/vnd.fdf", "fdf");
                _tipos.Add("text/x-fortran", "f");
                _tipos.Add("application/vnd.mif", "mif");
                _tipos.Add("application/vnd.framemaker", "fm");
                _tipos.Add("image/x-freehand", "fh");
                _tipos.Add("application/vnd.fsc.weblaunch", "fsc");
                _tipos.Add("application/vnd.frogans.fnc", "fnc");
                _tipos.Add("application/vnd.frogans.ltf", "ltf");
                _tipos.Add("application/vnd.fujixerox.ddd", "ddd");
                _tipos.Add("application/vnd.fujixerox.docuworks", "xdw");
                _tipos.Add("application/vnd.fujixerox.docuworks.binder", "xbd");
                _tipos.Add("application/vnd.fujitsu.oasys", "oas");
                _tipos.Add("application/vnd.fujitsu.oasys2", "oa2");
                _tipos.Add("application/vnd.fujitsu.oasys3", "oa3");
                _tipos.Add("application/vnd.fujitsu.oasysgp", "fg5");
                _tipos.Add("application/vnd.fujitsu.oasysprs", "bh2");
                _tipos.Add("application/x-futuresplash", "spl");
                _tipos.Add("application/vnd.fuzzysheet", "fzs");
                _tipos.Add("image/g3fax", "g3");
                _tipos.Add("application/vnd.gmx", "gmx");
                _tipos.Add("model/vnd.gtw", "gtw");
                _tipos.Add("application/vnd.genomatix.tuxedo", "txd");
                _tipos.Add("application/vnd.geogebra.file", "ggb");
                _tipos.Add("application/vnd.geogebra.tool", "ggt");
                _tipos.Add("model/vnd.gdl", "gdl");
                _tipos.Add("application/vnd.geometry-explorer", "gex");
                _tipos.Add("application/vnd.geonext", "gxt");
                _tipos.Add("application/vnd.geoplan", "g2w");
                _tipos.Add("application/vnd.geospace", "g3w");
                _tipos.Add("application/x-font-ghostscript", "gsf");
                _tipos.Add("application/x-font-bdf", "bdf");
                _tipos.Add("application/x-gtar", "gtar");
                _tipos.Add("application/x-texinfo", "texinfo");
                _tipos.Add("application/x-gnumeric", "gnumeric");
                _tipos.Add("application/vnd.google-earth.kml+xml", "kml");
                _tipos.Add("application/vnd.google-earth.kmz", "kmz");
                _tipos.Add("application/vnd.grafeq", "gqf");
                _tipos.Add("image/gif", "gif");
                _tipos.Add("text/vnd.graphviz", "gv");
                _tipos.Add("application/vnd.groove-account", "gac");
                _tipos.Add("application/vnd.groove-help", "ghf");
                _tipos.Add("application/vnd.groove-identity-message", "gim");
                _tipos.Add("application/vnd.groove-injector", "grv");
                _tipos.Add("application/vnd.groove-tool-message", "gtm");
                _tipos.Add("application/vnd.groove-tool-template", "tpl");
                _tipos.Add("application/vnd.groove-vcard", "vcg");
                _tipos.Add("video/h261", "h261");
                _tipos.Add("video/h263", "h263");
                _tipos.Add("video/h264", "h264");
                _tipos.Add("application/vnd.hp-hpid", "hpid");
                _tipos.Add("application/vnd.hp-hps", "hps");
                _tipos.Add("application/x-hdf", "hdf");
                _tipos.Add("audio/vnd.rip", "rip");
                _tipos.Add("application/vnd.hbci", "hbci");
                _tipos.Add("application/vnd.hp-jlyt", "jlt");
                _tipos.Add("application/vnd.hp-pcl", "pcl");
                _tipos.Add("application/vnd.hp-hpgl", "hpgl");
                _tipos.Add("application/vnd.yamaha.hv-script", "hvs");
                _tipos.Add("application/vnd.yamaha.hv-dic", "hvd");
                _tipos.Add("application/vnd.yamaha.hv-voice", "hvp");
                _tipos.Add("application/vnd.hydrostatix.sof-data", "sfd-hdstx");
                _tipos.Add("application/hyperstudio", "stk");
                _tipos.Add("application/vnd.hal+xml", "hal");
                _tipos.Add("text/html", "html");
                _tipos.Add("application/vnd.ibm.rights-management", "irm");
                _tipos.Add("application/vnd.ibm.secure-container", "sc");
                _tipos.Add("text/calendar", "ics");
                _tipos.Add("application/vnd.iccprofile", "icc");
                _tipos.Add("image/x-icon", "ico");
                _tipos.Add("application/vnd.igloader", "igl");
                _tipos.Add("image/ief", "ief");
                _tipos.Add("application/vnd.immervision-ivp", "ivp");
                _tipos.Add("application/vnd.immervision-ivu", "ivu");
                _tipos.Add("application/reginfo+xml", "rif");
                _tipos.Add("text/vnd.in3d.3dml", "3dml");
                _tipos.Add("text/vnd.in3d.spot", "spot");
                _tipos.Add("model/iges", "igs");
                _tipos.Add("application/vnd.intergeo", "i2g");
                _tipos.Add("application/vnd.cinderella", "cdy");
                _tipos.Add("application/vnd.intercon.formnet", "xpw");
                _tipos.Add("application/vnd.isac.fcs", "fcs");
                _tipos.Add("application/ipfix", "ipfix");
                _tipos.Add("application/pkix-cert", "cer");
                _tipos.Add("application/pkixcmp", "pki");
                _tipos.Add("application/pkix-crl", "crl");
                _tipos.Add("application/pkix-pkipath", "pkipath");
                _tipos.Add("application/vnd.insors.igm", "igm");
                _tipos.Add("application/vnd.ipunplugged.rcprofile", "rcprofile");
                _tipos.Add("application/vnd.irepository.package+xml", "irp");
                _tipos.Add("text/vnd.sun.j2me.app-descriptor", "jad");
                _tipos.Add("application/java-archive", "jar");
                _tipos.Add("application/java-vm", "class");
                _tipos.Add("application/x-java-jnlp-file", "jnlp");
                _tipos.Add("application/java-serialized-object", "ser");
                _tipos.Add("text/x-java-source);java", "java");
                _tipos.Add("application/javascript", "js");
                _tipos.Add("application/json", "json");
                _tipos.Add("application/vnd.joost.joda-archive", "joda");
                _tipos.Add("video/jpm", "jpm");
                _tipos.Add("image/x-citrix-jpeg", "jpeg");
                //tipos.Add("image/x-citrix-jpeg", "jpg");
                _tipos.Add("image/pjpeg", "pjpeg");
                _tipos.Add("video/jpeg", "jpgv");
                _tipos.Add("application/vnd.kahootz", "ktz");
                _tipos.Add("application/vnd.chipnuts.karaoke-mmd", "mmd");
                _tipos.Add("application/vnd.kde.karbon", "karbon");
                _tipos.Add("application/vnd.kde.kchart", "chrt");
                _tipos.Add("application/vnd.kde.kformula", "kfo");
                _tipos.Add("application/vnd.kde.kivio", "flw");
                _tipos.Add("application/vnd.kde.kontour", "kon");
                _tipos.Add("application/vnd.kde.kpresenter", "kpr");
                _tipos.Add("application/vnd.kde.kspread", "ksp");
                _tipos.Add("application/vnd.kde.kword", "kwd");
                _tipos.Add("application/vnd.kenameaapp", "htke");
                _tipos.Add("application/vnd.kidspiration", "kia");
                _tipos.Add("application/vnd.kinar", "kne");
                _tipos.Add("application/vnd.kodak-descriptor", "sse");
                _tipos.Add("application/vnd.las.las+xml", "lasxml");
                _tipos.Add("application/x-latex", "latex");
                _tipos.Add("application/vnd.llamagraphics.life-balance.desktop", "lbd");
                _tipos.Add("application/vnd.llamagraphics.life-balance.exchange+xml", "lbe");
                _tipos.Add("application/vnd.jam", "jam");
                _tipos.Add("application/vnd.lotus-1-2-3", "123");
                _tipos.Add("application/vnd.lotus-approach", "apr");
                _tipos.Add("application/vnd.lotus-freelance", "pre");
                _tipos.Add("application/vnd.lotus-notes", "nsf");
                _tipos.Add("application/vnd.lotus-organizer", "org");
                _tipos.Add("application/vnd.lotus-screencam", "scm");
                _tipos.Add("application/vnd.lotus-wordpro", "lwp");
                _tipos.Add("audio/vnd.lucent.voice", "lvp");
                _tipos.Add("audio/x-mpegurl", "m3u");
                _tipos.Add("video/x-m4v", "m4v");
                _tipos.Add("application/mac-binhex40", "hqx");
                _tipos.Add("application/vnd.macports.portpkg", "portpkg");
                _tipos.Add("application/vnd.osgeo.mapguide.package", "mgp");
                _tipos.Add("application/marc", "mrc");
                _tipos.Add("application/marcxml+xml", "mrcx");
                _tipos.Add("application/mxf", "mxf");
                _tipos.Add("application/vnd.wolfram.player", "nbp");
                _tipos.Add("application/mathematica", "ma");
                _tipos.Add("application/mathml+xml", "mathml");
                _tipos.Add("application/mbox", "mbox");
                _tipos.Add("application/vnd.medcalcdata", "mc1");
                _tipos.Add("application/mediaservercontrol+xml", "mscml");
                _tipos.Add("application/vnd.mediastation.cdkey", "cdkey");
                _tipos.Add("application/vnd.mfer", "mwf");
                _tipos.Add("application/vnd.mfmp", "mfm");
                _tipos.Add("model/mesh", "msh");
                _tipos.Add("application/mads+xml", "mads");
                //tipos.Add("application/mets+xml", "mets");
                //tipos.Add("application/mods+xml", "mods");
                _tipos.Add("application/metalink4+xml", "meta4");
                _tipos.Add("application/vnd.mcd", "mcd");
                _tipos.Add("application/vnd.micrografx.flo", "flo");
                _tipos.Add("application/vnd.micrografx.igx", "igx");
                _tipos.Add("application/vnd.eszigno3+xml", "es3");
                _tipos.Add("application/x-msaccess", "mdb");
                _tipos.Add("video/x-ms-asf", "asf");
                _tipos.Add("application/x-msdownload", "exe");
                _tipos.Add("application/vnd.ms-artgalry", "cil");
                _tipos.Add("application/vnd.ms-cab-compressed", "cab");
                _tipos.Add("application/vnd.ms-ims", "ims");
                _tipos.Add("application/x-ms-application", "application");
                _tipos.Add("application/x-msclip", "clp");
                _tipos.Add("image/vnd.ms-modi", "mdi");
                _tipos.Add("application/vnd.ms-fontobject", "eot");
                _tipos.Add("application/vnd.ms-excel", "xls");
                _tipos.Add("application/vnd.ms-excel.addin.macroenabled.12", "xlam");
                _tipos.Add("application/vnd.ms-excel.sheet.binary.macroenabled.12", "xlsb");
                _tipos.Add("application/vnd.ms-excel.template.macroenabled.12", "xltm");
                _tipos.Add("application/vnd.ms-excel.sheet.macroenabled.12", "xlsm");
                _tipos.Add("application/vnd.ms-htmlhelp", "chm");
                _tipos.Add("application/x-mscardfile", "crd");
                _tipos.Add("application/vnd.ms-lrm", "lrm");
                _tipos.Add("application/x-msmediaview", "mvb");
                _tipos.Add("application/x-msmoney", "mny");
                _tipos.Add("application/vnd.openxmlformats-officedocument.presentationml.presentation", "pptx");
                _tipos.Add("application/vnd.openxmlformats-officedocument.presentationml.slide", "sldx");
                _tipos.Add("application/vnd.openxmlformats-officedocument.presentationml.slideshow", "ppsx");
                _tipos.Add("application/vnd.openxmlformats-officedocument.presentationml.template", "potx");
                _tipos.Add("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "xlsx");
                _tipos.Add("application/vnd.openxmlformats-officedocument.spreadsheetml.template", "xltx");
                _tipos.Add("application/vnd.openxmlformats-officedocument.wordprocessingml.document", "docx");
                _tipos.Add("application/vnd.openxmlformats-officedocument.wordprocessingml.template", "dotx");
                _tipos.Add("application/x-msbinder", "obd");
                _tipos.Add("application/vnd.ms-officetheme", "thmx");
                _tipos.Add("application/onenote", "onetoc");
                _tipos.Add("audio/vnd.ms-playready.media.pya", "pya");
                _tipos.Add("video/vnd.ms-playready.media.pyv", "pyv");
                _tipos.Add("application/vnd.ms-powerpoint", "ppt");
                _tipos.Add("application/vnd.ms-powerpoint.addin.macroenabled.12", "ppam");
                _tipos.Add("application/vnd.ms-powerpoint.slide.macroenabled.12", "sldm");
                _tipos.Add("application/vnd.ms-powerpoint.presentation.macroenabled.12", "pptm");
                _tipos.Add("application/vnd.ms-powerpoint.slideshow.macroenabled.12", "ppsm");
                _tipos.Add("application/vnd.ms-powerpoint.template.macroenabled.12", "potm");
                _tipos.Add("application/vnd.ms-project", "mpp");
                _tipos.Add("application/x-mspublisher", "pub");
                _tipos.Add("application/x-msschedule", "scd");
                _tipos.Add("application/x-silverlight-app", "xap");
                _tipos.Add("application/vnd.ms-pki.stl", "stl");
                _tipos.Add("application/vnd.ms-pki.seccat", "cat");
                _tipos.Add("application/vnd.visio", "vsd");
                _tipos.Add("application/vnd.visio2013", "vsdx");
                _tipos.Add("video/x-ms-wm", "wm");
                _tipos.Add("audio/x-ms-wma", "wma");
                _tipos.Add("audio/x-ms-wax", "wax");
                _tipos.Add("video/x-ms-wmx", "wmx");
                _tipos.Add("application/x-ms-wmd", "wmd");
                _tipos.Add("application/vnd.ms-wpl", "wpl");
                _tipos.Add("application/x-ms-wmz", "wmz");
                _tipos.Add("video/x-ms-wmv", "wmv");
                _tipos.Add("video/x-ms-wvx", "wvx");
                _tipos.Add("application/x-msmetafile", "wmf");
                _tipos.Add("application/x-msterminal", "trm");
                _tipos.Add("application/msword", "doc");
                _tipos.Add("application/vnd.ms-word.document.macroenabled.12", "docm");
                _tipos.Add("application/vnd.ms-word.template.macroenabled.12", "dotm");
                _tipos.Add("application/x-mswrite", "wri");
                _tipos.Add("application/vnd.ms-works", "wps");
                _tipos.Add("application/x-ms-xbap", "xbap");
                _tipos.Add("application/vnd.ms-xpsdocument", "xps");
                _tipos.Add("audio/midi", "mid");
                _tipos.Add("application/vnd.ibm.minipay", "mpy");
                _tipos.Add("application/vnd.ibm.modcap", "afp");
                _tipos.Add("application/vnd.jcp.javame.midlet-rms", "rms");
                _tipos.Add("application/vnd.tmobile-livetv", "tmo");
                _tipos.Add("application/x-mobipocket-ebook", "prc");
                _tipos.Add("application/vnd.mobius.mbk", "mbk");
                _tipos.Add("application/vnd.mobius.dis", "dis");
                _tipos.Add("application/vnd.mobius.plc", "plc");
                _tipos.Add("application/vnd.mobius.mqy", "mqy");
                _tipos.Add("application/vnd.mobius.msl", "msl");
                _tipos.Add("application/vnd.mobius.txf", "txf");
                _tipos.Add("application/vnd.mobius.daf", "daf");
                _tipos.Add("text/vnd.fly", "fly");
                _tipos.Add("application/vnd.mophun.certificate", "mpc");
                _tipos.Add("application/vnd.mophun.application", "mpn");
                _tipos.Add("video/mj2", "mj2");
                //tipos.Add("audio/mpeg", "mpga");
                _tipos.Add("video/vnd.mpegurl", "mxu");
                _tipos.Add("video/mpeg", "mpeg");
                _tipos.Add("application/mp21", "m21");
                _tipos.Add("audio/mp4", "mp4a");
                _tipos.Add("video/mp4", "mp4");
                _tipos.Add("application/vnd.apple.mpegurl", "m3u8");
                _tipos.Add("application/vnd.musician", "mus");
                _tipos.Add("application/vnd.muvee.style", "msty");
                _tipos.Add("application/xv+xml", "mxml");
                _tipos.Add("application/vnd.nokia.n-gage.data", "ngdat");
                _tipos.Add("application/vnd.nokia.n-gage.symbian.install", "n-gage");
                _tipos.Add("application/x-dtbncx+xml", "ncx");
                _tipos.Add("application/x-netcdf", "nc");
                _tipos.Add("application/vnd.neurolanguage.nlu", "nlu");
                _tipos.Add("application/vnd.dna", "dna");
                _tipos.Add("application/vnd.noblenet-directory", "nnd");
                _tipos.Add("application/vnd.noblenet-sealer", "nns");
                _tipos.Add("application/vnd.noblenet-web", "nnw");
                _tipos.Add("application/vnd.nokia.radio-preset", "rpst");
                _tipos.Add("application/vnd.nokia.radio-presets", "rpss");
                _tipos.Add("text/n3", "n3");
                _tipos.Add("application/vnd.novadigm.edm", "edm");
                _tipos.Add("application/vnd.novadigm.edx", "edx");
                _tipos.Add("application/vnd.novadigm.ext", "ext");
                _tipos.Add("application/vnd.flographit", "gph");
                _tipos.Add("audio/vnd.nuera.ecelp4800", "ecelp4800");
                _tipos.Add("audio/vnd.nuera.ecelp7470", "ecelp7470");
                _tipos.Add("audio/vnd.nuera.ecelp9600", "ecelp9600");
                _tipos.Add("application/oda", "oda");
                _tipos.Add("application/ogg", "ogx");
                _tipos.Add("audio/ogg", "oga");
                _tipos.Add("video/ogg", "ogv");
                _tipos.Add("application/vnd.oma.dd2+xml", "dd2");
                _tipos.Add("application/vnd.oasis.opendocument.text-web", "oth");
                _tipos.Add("application/oebps-package+xml", "opf");
                _tipos.Add("application/vnd.intu.qbo", "qbo");
                _tipos.Add("application/vnd.openofficeorg.extension", "oxt");
                _tipos.Add("application/vnd.yamaha.openscoreformat", "osf");
                _tipos.Add("audio/webm", "weba");
                _tipos.Add("video/webm", "webm");
                _tipos.Add("application/vnd.oasis.opendocument.chart", "odc");
                _tipos.Add("application/vnd.oasis.opendocument.chart-template", "otc");
                _tipos.Add("application/vnd.oasis.opendocument.database", "odb");
                _tipos.Add("application/vnd.oasis.opendocument.formula", "odf");
                _tipos.Add("application/vnd.oasis.opendocument.formula-template", "odft");
                _tipos.Add("application/vnd.oasis.opendocument.graphics", "odg");
                _tipos.Add("application/vnd.oasis.opendocument.graphics-template", "otg");
                _tipos.Add("application/vnd.oasis.opendocument.image", "odi");
                _tipos.Add("application/vnd.oasis.opendocument.image-template", "oti");
                _tipos.Add("application/vnd.oasis.opendocument.presentation", "odp");
                _tipos.Add("application/vnd.oasis.opendocument.presentation-template", "otp");
                _tipos.Add("application/vnd.oasis.opendocument.spreadsheet", "ods");
                _tipos.Add("application/vnd.oasis.opendocument.spreadsheet-template", "ots");
                _tipos.Add("application/vnd.oasis.opendocument.text", "odt");
                _tipos.Add("application/vnd.oasis.opendocument.text-master", "odm");
                _tipos.Add("application/vnd.oasis.opendocument.text-template", "ott");
                _tipos.Add("image/ktx", "ktx");
                _tipos.Add("application/vnd.sun.xml.calc", "sxc");
                _tipos.Add("application/vnd.sun.xml.calc.template", "stc");
                _tipos.Add("application/vnd.sun.xml.draw", "sxd");
                _tipos.Add("application/vnd.sun.xml.draw.template", "std");
                _tipos.Add("application/vnd.sun.xml.impress", "sxi");
                _tipos.Add("application/vnd.sun.xml.impress.template", "sti");
                _tipos.Add("application/vnd.sun.xml.math", "sxm");
                _tipos.Add("application/vnd.sun.xml.writer", "sxw");
                _tipos.Add("application/vnd.sun.xml.writer.global", "sxg");
                _tipos.Add("application/vnd.sun.xml.writer.template", "stw");
                _tipos.Add("application/x-font-otf", "otf");
                _tipos.Add("application/vnd.yamaha.openscoreformat.osfpvg+xml", "osfpvg");
                _tipos.Add("application/vnd.osgi.dp", "dp");
                _tipos.Add("application/vnd.palm", "pdb");
                _tipos.Add("text/x-pascal", "p");
                _tipos.Add("application/vnd.pawaafile", "paw");
                _tipos.Add("application/vnd.hp-pclxl", "pclxl");
                _tipos.Add("application/vnd.picsel", "efif");
                _tipos.Add("image/x-pcx", "pcx");
                _tipos.Add("image/vnd.adobe.photoshop", "psd");
                _tipos.Add("application/pics-rules", "prf");
                _tipos.Add("image/x-pict", "pic");
                _tipos.Add("application/x-chat", "chat");
                _tipos.Add("application/pkcs10", "p10");
                _tipos.Add("application/x-pkcs12", "p12");
                _tipos.Add("application/pkcs7-mime", "p7m");
                _tipos.Add("application/pkcs7-signature", "p7s");
                _tipos.Add("application/x-pkcs7-certreqresp", "p7r");
                _tipos.Add("application/x-pkcs7-certificates", "p7b");
                _tipos.Add("application/pkcs8", "p8");
                _tipos.Add("application/vnd.pocketlearn", "plf");
                _tipos.Add("image/x-portable-anymap", "pnm");
                _tipos.Add("image/x-portable-bitmap", "pbm");
                _tipos.Add("application/x-font-pcf", "pcf");
                _tipos.Add("application/font-tdpfr", "pfr");
                _tipos.Add("application/x-chess-pgn", "pgn");
                _tipos.Add("image/x-portable-graymap", "pgm");
                _tipos.Add("image/x-png", "png");
                _tipos.Add("image/x-portable-pixmap", "ppm");
                _tipos.Add("application/pskc+xml", "pskcxml");
                _tipos.Add("application/vnd.ctc-posml", "pml");
                _tipos.Add("application/postscript", "ai");
                _tipos.Add("application/x-font-type1", "pfa");
                _tipos.Add("application/vnd.powerbuilder6", "pbd");
                _tipos.Add("application/pgp-signature", "pgp");
                _tipos.Add("application/vnd.previewsystems.box", "box");
                _tipos.Add("application/vnd.pvi.ptid1", "ptid");
                _tipos.Add("application/pls+xml", "pls");
                _tipos.Add("application/vnd.pg.format", "str");
                _tipos.Add("application/vnd.pg.osasli", "ei6");
                _tipos.Add("text/prs.lines.tag", "dsc");
                _tipos.Add("application/x-font-linux-psf", "psf");
                _tipos.Add("application/vnd.publishare-delta-tree", "qps");
                _tipos.Add("application/vnd.pmi.widget", "wg");
                _tipos.Add("application/vnd.quark.quarkxpress", "qxd");
                _tipos.Add("application/vnd.epson.esf", "esf");
                _tipos.Add("application/vnd.epson.msf", "msf");
                _tipos.Add("application/vnd.epson.ssf", "ssf");
                _tipos.Add("application/vnd.epson.quickanime", "qam");
                _tipos.Add("application/vnd.intu.qfx", "qfx");
                _tipos.Add("video/quicktime", "qt");
                _tipos.Add("application/x-rar-compressed", "rar");
                _tipos.Add("audio/x-pn-realaudio", "ram");
                _tipos.Add("audio/x-pn-realaudio-plugin", "rmp");
                _tipos.Add("application/rsd+xml", "rsd");
                _tipos.Add("application/vnd.rn-realmedia", "rm");
                _tipos.Add("application/vnd.realvnc.bed", "bed");
                _tipos.Add("application/vnd.recordare.musicxml", "mxl");
                _tipos.Add("application/vnd.recordare.musicxml+xml", "musicxml");
                _tipos.Add("application/relax-ng-compact-syntax", "rnc");
                _tipos.Add("application/vnd.data-vision.rdz", "rdz");
                _tipos.Add("application/rdf+xml", "rdf");
                _tipos.Add("application/vnd.cloanto.rp9", "rp9");
                _tipos.Add("application/vnd.jisp", "jisp");
                _tipos.Add("application/rtf", "rtf");
                _tipos.Add("text/richtext", "rtx");
                _tipos.Add("application/vnd.route66.link66+xml", "link66");
                _tipos.Add("application/rss+xml", "rss");
                _tipos.Add("application/shf+xml", "shf");
                _tipos.Add("application/vnd.sailingtracker.track", "st");
                _tipos.Add("image/svg+xml", "svg");
                _tipos.Add("application/vnd.sus-calendar", "sus");
                _tipos.Add("application/sru+xml", "sru");
                _tipos.Add("application/set-payment-initiation", "setpay");
                _tipos.Add("application/set-registration-initiation", "setreg");
                _tipos.Add("application/vnd.sema", "sema");
                _tipos.Add("application/vnd.semd", "semd");
                _tipos.Add("application/vnd.semf", "semf");
                _tipos.Add("application/vnd.seemail", "see");
                _tipos.Add("application/x-font-snf", "snf");
                _tipos.Add("application/scvp-vp-request", "spq");
                _tipos.Add("application/scvp-vp-response", "spp");
                _tipos.Add("application/scvp-cv-request", "scq");
                _tipos.Add("application/scvp-cv-response", "scs");
                _tipos.Add("application/sdp", "sdp");
                _tipos.Add("text/x-setext", "etx");
                _tipos.Add("video/x-sgi-movie", "movie");
                _tipos.Add("application/vnd.shana.informed.formdata", "ifm");
                _tipos.Add("application/vnd.shana.informed.formtemplate", "itp");
                _tipos.Add("application/vnd.shana.informed.interchange", "iif");
                _tipos.Add("application/vnd.shana.informed.package", "ipk");
                _tipos.Add("application/thraud+xml", "tfi");
                _tipos.Add("application/x-shar", "shar");
                _tipos.Add("image/x-rgb", "rgb");
                _tipos.Add("application/vnd.epson.salt", "slt");
                _tipos.Add("application/vnd.accpac.simply.aso", "aso");
                _tipos.Add("application/vnd.accpac.simply.imp", "imp");
                _tipos.Add("application/vnd.simtech-mindmapper", "twd");
                _tipos.Add("application/vnd.commonspace", "csp");
                _tipos.Add("application/vnd.yamaha.smaf-audio", "saf");
                _tipos.Add("application/vnd.smaf", "mmf");
                _tipos.Add("application/vnd.yamaha.smaf-phrase", "spf");
                _tipos.Add("application/vnd.smart.teacher", "teacher");
                _tipos.Add("application/vnd.svd", "svd");
                _tipos.Add("application/sparql-query", "rq");
                _tipos.Add("application/sparql-results+xml", "srx");
                _tipos.Add("application/srgs", "gram");
                _tipos.Add("application/srgs+xml", "grxml");
                _tipos.Add("application/ssml+xml", "ssml");
                _tipos.Add("application/vnd.koan", "skp");
                _tipos.Add("text/sgml", "sgml");
                _tipos.Add("application/vnd.stardivision.calc", "sdc");
                _tipos.Add("application/vnd.stardivision.draw", "sda");
                _tipos.Add("application/vnd.stardivision.impress", "sdd");
                _tipos.Add("application/vnd.stardivision.math", "smf");
                _tipos.Add("application/vnd.stardivision.writer", "sdw");
                _tipos.Add("application/vnd.stardivision.writer-global", "sgl");
                _tipos.Add("application/vnd.stepmania.stepchart", "sm");
                _tipos.Add("application/x-stuffit", "sit");
                _tipos.Add("application/x-stuffitx", "sitx");
                _tipos.Add("application/vnd.solent.sdkm+xml", "sdkm");
                _tipos.Add("application/vnd.olpc-sugar", "xo");
                _tipos.Add("audio/basic", "au");
                _tipos.Add("application/vnd.wqd", "wqd");
                _tipos.Add("application/vnd.symbian.install", "sis");
                _tipos.Add("application/smil+xml", "smi");
                _tipos.Add("application/vnd.syncml+xml", "xsm");
                _tipos.Add("application/vnd.syncml.dm+wbxml", "bdm");
                _tipos.Add("application/vnd.syncml.dm+xml", "xdm");
                _tipos.Add("application/x-sv4cpio", "sv4cpio");
                _tipos.Add("application/x-sv4crc", "sv4crc");
                _tipos.Add("application/sbml+xml", "sbml");
                _tipos.Add("text/tab-separated-values", "tsv");
                _tipos.Add("image/tiff", "tiff");
                _tipos.Add("application/vnd.tao.intent-module-archive", "tao");
                _tipos.Add("application/x-tar", "tar");
                _tipos.Add("application/x-tcl", "tcl");
                _tipos.Add("application/x-tex", "tex");
                _tipos.Add("application/x-tex-tfm", "tfm");
                _tipos.Add("application/tei+xml", "tei");
                _tipos.Add("text/plain", "txt");
                _tipos.Add("application/vnd.spotfire.dxp", "dxp");
                _tipos.Add("application/vnd.spotfire.sfs", "sfs");
                _tipos.Add("application/timestamped-data", "tsd");
                _tipos.Add("application/vnd.trid.tpt", "tpt");
                _tipos.Add("application/vnd.triscape.mxs", "mxs");
                _tipos.Add("text/troff", "t");
                _tipos.Add("application/vnd.trueapp", "tra");
                _tipos.Add("application/x-font-ttf", "ttf");
                _tipos.Add("text/turtle", "ttl");
                _tipos.Add("application/vnd.umajin", "umj");
                _tipos.Add("application/vnd.uoml+xml", "uoml");
                _tipos.Add("application/vnd.unity", "unityweb");
                _tipos.Add("application/vnd.ufdl", "ufd");
                _tipos.Add("text/uri-list", "uri");
                _tipos.Add("application/vnd.uiq.theme", "utz");
                _tipos.Add("application/x-ustar", "ustar");
                _tipos.Add("text/x-uuencode", "uu");
                _tipos.Add("text/x-vcalendar", "vcs");
                _tipos.Add("text/x-vcard", "vcf");
                _tipos.Add("application/x-cdlink", "vcd");
                _tipos.Add("application/vnd.vsf", "vsf");
                _tipos.Add("model/vrml", "wrl");
                _tipos.Add("application/vnd.vcx", "vcx");
                _tipos.Add("model/vnd.mts", "mts");
                _tipos.Add("model/vnd.vtu", "vtu");
                _tipos.Add("application/vnd.visionary", "vis");
                _tipos.Add("video/vnd.vivo", "viv");
                _tipos.Add("application/ccxml+xml);", "ccxml");
                _tipos.Add("application/voicexml+xml", "vxml");
                _tipos.Add("application/x-wais-source", "src");
                _tipos.Add("application/vnd.wap.wbxml", "wbxml");
                _tipos.Add("image/vnd.wap.wbmp", "wbmp");
                _tipos.Add("audio/x-wav", "wav");
                _tipos.Add("application/davmount+xml", "davmount");
                _tipos.Add("application/x-font-woff", "woff");
                _tipos.Add("application/wspolicy+xml", "wspolicy");
                _tipos.Add("image/webp", "webp");
                _tipos.Add("application/vnd.webturbo", "wtb");
                _tipos.Add("application/widget", "wgt");
                _tipos.Add("application/winhlp", "hlp");
                _tipos.Add("text/vnd.wap.wml", "wml");
                _tipos.Add("text/vnd.wap.wmlscript", "wmls");
                _tipos.Add("application/vnd.wap.wmlscriptc", "wmlsc");
                _tipos.Add("application/vnd.wordperfect", "wpd");
                _tipos.Add("application/vnd.wt.stf", "stf");
                _tipos.Add("application/wsdl+xml", "wsdl");
                _tipos.Add("image/x-xbitmap", "xbm");
                _tipos.Add("image/x-xpixmap", "xpm");
                _tipos.Add("image/x-xwindowdump", "xwd");
                _tipos.Add("application/x-x509-ca-cert", "der");
                _tipos.Add("application/x-xfig", "fig");
                _tipos.Add("application/xhtml+xml", "xhtml");
                _tipos.Add("application/xml", "xml");
                _tipos.Add("application/xcap-diff+xml", "xdf");
                _tipos.Add("application/xenc+xml", "xenc");
                _tipos.Add("application/patch-ops-error+xml", "xer");
                _tipos.Add("application/resource-lists+xml", "rl");
                _tipos.Add("application/rls-services+xml", "rs");
                _tipos.Add("application/resource-lists-diff+xml", "rld");
                _tipos.Add("application/xslt+xml", "xslt");
                _tipos.Add("application/xop+xml", "xop");
                _tipos.Add("application/x-xpinstall", "xpi");
                _tipos.Add("application/xspf+xml", "xspf");
                _tipos.Add("application/vnd.mozilla.xul+xml", "xul");
                _tipos.Add("chemical/x-xyz", "xyz");
                _tipos.Add("text/yaml", "yaml");
                _tipos.Add("application/yang", "yang");
                _tipos.Add("application/yin+xml", "yin");
                _tipos.Add("application/vnd.zul", "zir");
                _tipos.Add("application/zip", "zip");
                _tipos.Add("application/x-zip-compressed", "zip");
                _tipos.Add("application/vnd.handheld-entertainment+xml", "zmm");
                _tipos.Add("application/vnd.zzazz.deck+xml", "zaz");
                _tipos.Add("application/vnd.sqlite3", "db");
                #endregion
            }
            var result = "unknown";
            if (_tipos.ContainsKey(tipo))
            {
                result = _tipos[tipo];
            }
            return result;
        }
        
        public virtual void LimparArquivos()
        {
            var dateTime = DateTimeSync.Now.AddHours(-1);
            ArquivoRepository.GetAll()
                .Where(i => i.Temporario && i.DataDeCriacao < dateTime)
                .ToArray()
                .ForEach(i =>
                {
                    SafeExecute(() =>
                    {
                        ArquivoRepository.Delete(i);
                        var fileName = GetFilePath(i);
                        File.Delete(fileName);
                    });
                });
        }

        private void SafeExecute(Action action)
        {
            var startTime = DateTimeSync.Now;
            try
            {
                action();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{this.GetType().Name}: {e.ToString()}");
                try
                {
                    log.Logger.Add(
                        Log.Modulo.WebApi.Description(),
                         this.GetType().Name,
                        null,
                        e.ToString(),
                        Log.Categoria.Geral.ToInt(),
                        Log.SubCategoria.Erro.ToInt(),
                        null,
                        null,
                        "127.0.0.1",
                        startTime,
                        null,
                        null);
                }
                catch (System.Exception e2)
                {
                    Console.WriteLine($"Erro ao gravar log: {e2.ToString()}");
                }
            }
        }
    }
}
