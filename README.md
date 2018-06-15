# GitSharp
Simplified multi-platform implementation of Git in C#.

## Build

## Architecture
Is based on existing Git architecture.
For some reference about git architecture see Git Internals in [Git Pro Book](https://git-scm.com/book/en/v2/Git-Internals-Plumbing-and-Porcelain)
and [Hackernnon - understanding Git data model](https://hackernoon.com/https-medium-com-zspajich-understanding-git-data-model-95eb16cc99f5)

Following text will mention some Git Internals principles that are used in GitSharp solution
and link those principles with some of the classes.

### Object types
There are three object types in git - blob, tree and commit.
They are used in two different ways.
They may be either loaded ie. read from a file in objects directory, or constructed,
filled with some values and stored ie. written to a file.

In GitSharp implementation object files are read-only.
When you read an object, you have some immutable type, those are classes
that implements `Objects.IGitObject` interface.
On the other hand, when you want to construct an object and then store it (for example `Objects.TreeBuilder`)
you have to implement `Objects.IStorableGitObject`.

Note that loading and storing of object files is done solely via `ObjectDatabase`
that is mentioned bellow.

#### Blob
Blob represents content of some file in given time - blob is basically a snapshot of file.
`Objects.Blob` is a type that may be used for both creating and storing blobs and
loading blobs.

#### Tree
Tree represents a snapshot of a directory.
Tree may contain other trees (subdirectories) and blobs (files inside directory).
`Objects.Tree` represents a lodeable tree object.
All the subobjects (ie. subtrees and blobs) may be loaded on-demand via `Load` methods.
Note that it would be inefficient to implement `Objects.Tree` to load all
the subobjects during construction, because there may be a lot of subobjects.
`Objects.TreeBuilder` represents a writeable tree object for constructing a tree.

#### Commit
Commit represents a snapshot of the whole repository.
In GitSharp implementation tt contains just root tree, parent commit and message.
`Objects.Commit` represents a commit object and both root tree and parent commit
may be loaded-on demand via `Load` methods (same principle as in `Objects.Tree`).

### ObjectDatabase
`ObjectDatabase` is used for storing and retrieving git objects.
It is implemented as a static class, like `Index` and `ReferenceDatabase`.
Every git object is stored under a key (SHA-1 `Hash.HashKey`) and retrieved
via that same key.
Generation of that key is simple - SHA-1 is used on a file which is about to save.

### Index
Index is the most important data structure.
It keeps track of every file in working directory.
Note that the files that there may be untracked files in working directory that
are not contained in index.
Index is one file and static `Index` class reads the whole file when necessary and
in the end of the program writes changes into this file.
`Index` class contains all the methods that are necessary to manipulate the index,
for more information see the documentation of the class.
Note that the GitSharp index format is completely different than Git index format.

### ReferenceDatabase
`ReferenceDatabase` is a static class that keeps track of all the branches and HEAD.

### Commands
The core functionality of GitSharp is that the user input is parsed into a `Command` and
its `Process` method is called.
After `Process` method finishes, `Index` and `ReferenceDatabase` are disposed.
Every currently supported command is in `Commands` namespace.
Option parsing is responsibility of every command.


## Tests

## Remarks
Any configuration files are currently not supported.
For visual and debugging reasons, object files are not compressed (as opposed to Git),
and are human-readable.
Any compressing algorithm may be added directly to `ObjectDatabase` and its
`Retrieve` and `Store` methods.

## Usage
Supported commands are subset of existing Git commands - for more information see Git documentation.

`git_sharp init`