import subprocess
import os
import shutil
from filecmp import dircmp

VERBOSE = True
EXIT_AFTER_FIRST_ERROR = True

GIT_EXEC = 'git'
GIT_ROOT_DIR = '/tmp/GIT_ROOT_DIR'

GIT_SHARP_EXEC = '/home/mayfa/workspaces/rider/GitSharp/GitSharp/bin/Debug/GitSharp.exe'
GIT_SHARP_ROOT_DIR = '/tmp/GIT_SHARP_ROOT_DIR'


class Command:
    @staticmethod
    def join_args(args: [str]) -> str:
        res_str = ''
        for arg in args:
            res_str += arg
            res_str += ' '
        return res_str

    @staticmethod
    def run(args: [str]) -> None:
        if VERBOSE:
            print('running: ' + Command.join_args(args))

        process = subprocess.run(args)
        if process.returncode != 0:
            print('command ' + Command.join_args(args) + ' FAILED, exiting...')
            script_exit()


class InitCommand(Command):
    def process(self) -> None:
        os.chdir(GIT_ROOT_DIR)
        Command.run([GIT_EXEC, 'init'])

        os.chdir(GIT_SHARP_ROOT_DIR)
        Command.run([GIT_SHARP_EXEC, 'init'])


class CommitCommand(Command):
    def __init__(self, message: str):
        self.message = message

    def process(self) -> None:
        os.chdir(GIT_ROOT_DIR)
        Command.run([GIT_EXEC, 'commit', '-m "' + self.message + '"'])

        os.chdir(GIT_SHARP_ROOT_DIR)
        Command.run([GIT_SHARP_EXEC, 'commit', '"' + self.message + '"'])
        check_root_directories_equality()


class AddCommand(Command):
    def __init__(self, files: [str]):
        self.files = files

    def process(self) -> None:
        os.chdir(GIT_ROOT_DIR)
        Command.run([GIT_EXEC, 'add'] + self.files)

        os.chdir(GIT_SHARP_ROOT_DIR)
        Command.run([GIT_SHARP_EXEC, 'add'] + self.files)
        check_root_directories_equality()


class BranchCommand(Command):
    def __init__(self, branch_name: str):
        self.branch_name = branch_name

    def process(self) -> None:
        os.chdir(GIT_ROOT_DIR)
        Command.run([GIT_EXEC, 'branch', self.branch_name])

        os.chdir(GIT_SHARP_ROOT_DIR)
        Command.run([GIT_SHARP_EXEC, 'branch', self.branch_name])
        check_root_directories_equality()


class CheckoutCommand(Command):
    def __init__(self, branch_name: str):
        self.branch_name = branch_name

    def process(self) -> None:
        os.chdir(GIT_ROOT_DIR)
        Command.run([GIT_EXEC, 'checkout', self.branch_name])

        os.chdir(GIT_SHARP_ROOT_DIR)
        Command.run([GIT_SHARP_EXEC, 'checkout', self.branch_name])
        check_root_directories_equality()


class CreateFileCommand(Command):
    def __init__(self, file_name: str):
        self.file_name = file_name

    def process(self) -> None:
        if VERBOSE:
            print('Creating file ' + self.file_name)
        file = open(os.path.join(GIT_ROOT_DIR, self.file_name), 'w')
        file.close()
        file = open(os.path.join(GIT_SHARP_ROOT_DIR, self.file_name), 'w')
        file.close()
        check_root_directories_equality()


class ModifyFileCommand(Command):
    def __init__(self, file_name: str, file_content: str):
        self.file_name = file_name
        self.file_content = file_content

    def process(self) -> None:
        if VERBOSE:
            print('Modifying file ' + self.file_name)
        file = open(os.path.join(GIT_ROOT_DIR, self.file_name), 'w')
        file.write(self.file_content)
        file.close()
        file = open(os.path.join(GIT_SHARP_ROOT_DIR, self.file_name), 'w')
        file.write(self.file_content)
        file.close()
        check_root_directories_equality()


def check_root_directories_equality() -> None:
    dir_cmp = dircmp(GIT_ROOT_DIR, GIT_SHARP_ROOT_DIR, ignore=['.git', '.git_sharp'])
    if len(dir_cmp.diff_files) > 0:
        print('===== Directories differ =====')
        print('Different files: ' + dir_cmp.diff_files)
        print('==============================')
        script_exit()
    pass


def script_exit() -> None:
    if EXIT_AFTER_FIRST_ERROR:
        exit(1)


def init_directories() -> None:
    if os.path.exists(GIT_ROOT_DIR):
        shutil.rmtree(GIT_ROOT_DIR)
    if os.path.exists(GIT_SHARP_ROOT_DIR):
        shutil.rmtree(GIT_SHARP_ROOT_DIR)
    os.mkdir(GIT_ROOT_DIR)
    os.mkdir(GIT_SHARP_ROOT_DIR)

    InitCommand().process()


def process_commands(cmds: [Command]) -> None:
    for cmd in cmds:
        cmd.process()


simple_commands = [
    CreateFileCommand('a.txt'),
    AddCommand(['a.txt']),
    CommitCommand('Initial commit'),
    BranchCommand('branch-1'),

    CreateFileCommand('b.txt'),
    ModifyFileCommand('b.txt', 'b-content'),
    AddCommand(['b.txt']),
    CommitCommand('Master commit'),

    CheckoutCommand('branch-1')
]

initial_commit_commands = [
    CreateFileCommand('z.txt'),
    AddCommand(['z.txt']),
    CommitCommand('Initial commit')
]


def many_branches_test() -> None:
    """
    Creates many branches, adds a commit to every branch and then checks out
    every branch
    """
    command_list = []
    command_list += initial_commit_commands
    for i in range(10):
        branch_name = 'branch-' + str(i)
        command_list.append(BranchCommand(branch_name))
        command_list.append(CheckoutCommand(branch_name))
        command_list.append(CreateFileCommand('a.txt'))
        command_list.append(ModifyFileCommand('a.txt', branch_name + ' content'))
        command_list.append(AddCommand(['a.txt']))
        command_list.append(CommitCommand(branch_name + ' commit'))
        command_list.append(CheckoutCommand('master'))

    for i in range(10):
        branch_name = 'branch-' + str(i)
        command_list.append(CheckoutCommand(branch_name))

    process_commands(command_list)


def many_files_test() -> None:
    command_list = []
    command_list += initial_commit_commands

    # Create many files and stage half of them
    for i in range(200):
        file_name = 'file-' + str(i)
        file_content = 'file-' + str(i) + ' content'
        command_list.append(CreateFileCommand(file_name))
        command_list.append(ModifyFileCommand(file_name, file_content))
        if i % 2 == 0:
            command_list.append(AddCommand([file_name]))

    command_list.append(CommitCommand('Big commit'))
    command_list.append(BranchCommand('big-branch'))

    # Create many files and stage half of them
    for i in range(120):
        file_name = 'file-' + str(i)
        file_content = 'file-' + str(i) + ' content'
        command_list.append(CreateFileCommand(file_name))
        command_list.append(ModifyFileCommand(file_name, file_content))
        if i % 1 == 0:
            command_list.append(AddCommand([file_name]))

    command_list.append(CommitCommand('Second big commit'))
    command_list.append(BranchCommand('second-big-branch'))

    command_list.append(CheckoutCommand('big-branch'))
    command_list.append(CheckoutCommand('second-big-branch'))
    command_list.append(CheckoutCommand('big-branch'))
    command_list.append(CheckoutCommand('master'))

    process_commands(command_list)


init_directories()
many_branches_test()

init_directories()
many_files_test()
