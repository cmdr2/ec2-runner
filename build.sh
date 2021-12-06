rm -r Builds/Parallel

/Applications/Unity/Hub/Editor/2021.1.17f1/Unity.app/Contents/MacOS/Unity -batchmode -quit -executeMethod ScriptedBuild.PerformHeadlessWindowsBuild

cd Builds/
echo $(date) > Parallel/build-date.txt
zip -q -r Parallel.zip Parallel
