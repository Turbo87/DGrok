TEMP_DIR = ENV['TMP'].gsub('\\', '/')
IMAGE_DIR = File.join(TEMP_DIR, 'DGrokImage')
IMAGE_SOURCE = File.join(IMAGE_DIR, 'Source')
ZIP_EXE = "c:/program files/7-Zip/7z.exe"
version_source = IO.read("DGrok.Framework/Properties/AssemblyVersion.cs")
version = /AssemblyVersion\(\"([^"]+)\"/.match(version_source)[1]
CODE_VERSION = version.sub(/(\.0){1,2}$/, "")
RELEASES_DIR = File.expand_path("Releases")
ZIP_NAME = File.join(RELEASES_DIR, "DGrok-#{CODE_VERSION}.zip")

file 'Grammar.html' => ['Grammar.yaml', 'MakePrettyGrammarHtml.rb'] do
  ruby 'MakePrettyGrammarHtml.rb'
end

task :verify_license_headers do
  notice = IO.read("osl-notice.txt")
  files_to_check = FileList.new("**/*.cs")
  files_to_check = files_to_check.exclude("NUnitLite/**")
  files_to_check = files_to_check.exclude("**/*.Designer.*")
  files_to_check.each do |filename|
    contents = IO.read(filename)
    if !contents.include?(notice)
      fail "File '#{filename}' does not include OSL license header"
    end
  end
end

task :codegen do
  ruby 'GenerateCode.rb'
end

task :build => [:verify_license_headers, :codegen] do
  sh "msbuild /verbosity:quiet /nologo /p:Configuration=Release"
end

task :tests => :build do
  sh "Bin/DGrok.Tests.exe"
end

def upload(local_filename, remote_dir, selector)
  require 'net/ftp'
  require 'yaml'
  info = YAML.load_file("c:/svn/dgrok-upload.yaml")
  Net::FTP.open(info['server']) do |ftp|
    ftp.login(info['login'], info['password'])
    ftp.chdir(remote_dir)
    ftp.__send__(selector, local_filename)
  end
end

task :upload_grammar => 'Grammar.html' do
  print "Uploading grammar doc... "
  STDOUT.flush
  upload('Grammar.html', 'dgrok', :puttextfile)
  puts "Done."
end

task :default => ['Grammar.html', :tests] do
  puts "Done"
end

task :commit => :default do
  print "Opening TortoiseSVN commit dialog... "
  STDOUT.flush
  system %Q|"C:/Program Files/TortoiseSVN/bin/TortoiseProc.exe" | +
    %Q|/command:commit /path:"#{Dir.getwd}" /notempfile|
  puts "Done."
end

task :image => :default do
  rm_rf IMAGE_DIR
  mkdir_p IMAGE_DIR
  sh %Q|svn export . "#{IMAGE_SOURCE}"|
  cp "DGrok.Framework/DelphiNodes/GeneratedNodes.cs", File.join(IMAGE_SOURCE, "DGrok.Framework/DelphiNodes")
  cp "DGrok.Framework/Framework/GeneratedVisitor.cs", File.join(IMAGE_SOURCE, "DGrok.Framework/Framework")
  cp ["Grammar.html", "Grammar.css", "README.html"], IMAGE_DIR
  cp_r Dir["Bin/*"], IMAGE_DIR
end

task :zip => :image do
  mkdir_p RELEASES_DIR
  if File.exist?(ZIP_NAME)
    if ENV['FORCE'] == '1'
      rm_f ZIP_NAME
    else
      fail "Release '#{File.basename(ZIP_NAME)}' already exists. Specify FORCE=1 to overwrite."
    end
  end
  sh %Q|"#{ZIP_EXE}" a -tzip -r #{ZIP_NAME} "#{File.join(IMAGE_DIR, '*')}"|
end

task :upload_zip do
  print "Uploading release zip... "
  STDOUT.flush
  upload(ZIP_NAME, 'dgrok', :putbinaryfile)
  puts "Done."
end

task :tag do
  info = `svn info`
  trunk_url = /^URL:\s+(\S*)/.match(info)[1]
  tag_url = trunk_url.sub(/\/trunk\b/, "/tags/Release-#{CODE_VERSION}")
  sh %Q|svn cp #{trunk_url} #{tag_url} -m "Tagging release #{CODE_VERSION}"|
end

task :release => [:upload_grammar, :zip, :upload_zip, :tag] do
  puts "Release completed."
end