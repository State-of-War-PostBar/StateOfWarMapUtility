test: build
	dotnet build ./Test/x.csproj
	./Test/bin/StateOfWarUtility.Test.exe
	
build:
	dotnet build ./Library/x.csproj
