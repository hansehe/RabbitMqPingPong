from DockerBuildSystem import TerminalTools
from SwarmManagement import SwarmTools
import os
import sys

PROJECT_PATH = 'RabbitMqPingPong/Services/RabbitMqPingPong.Service/RabbitMqPingPong.Service.csproj'
RUNTIME = 'win-x64'
OUTPUT_FOLDER = 'output/'


def PublishService(projectPath, runtime, outputFolder):
    terminalCommand = 'dotnet publish %s -c release -r %s -o %s' % (projectPath, runtime, outputFolder)
    TerminalTools.ExecuteTerminalCommands([terminalCommand], True)


def GetOutputPathFromArguments(arguments):
    outputPath = SwarmTools.GetArgumentValues(arguments, '-o')
    if len(outputPath) == 0:
        return OUTPUT_FOLDER
    return outputPath[0]


def GetRuntimeFromArguments(arguments):
    runtime = SwarmTools.GetArgumentValues(arguments, '-r')
    if len(runtime) == 0:
        return RUNTIME
    return runtime[0]


if __name__ == "__main__":
    arguments = sys.argv
    runtime = GetRuntimeFromArguments(arguments)
    outputFolder = GetOutputPathFromArguments(arguments)
    outputFolder = os.path.join(os.getcwd(), outputFolder)
    PublishService(PROJECT_PATH, runtime, outputFolder)
