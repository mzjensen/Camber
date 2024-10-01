# Contributing to Camber

Camber is an open source project. This means that it is open to contributions from anyone. Any help is appreciated!

## Filing issues

When filing an issue, make sure to answer these five questions:

1. Which version of Dynamo and Civil 3D are you using (check the About box)?
2. Which operating system are you using?
3. What did you do?
4. What did you expect to see?
5. What did you see instead?

General questions about using Camber should be submitted to [the forum at dynamobim.org](https://forum.dynamobim.com/t/camber-feedback-thread/68942/).

## Contributing code

Please follow this template before submitting a pull request.

```
### Purpose

(FILL ME IN) This section describes why this PR is here.
Usually it would include a reference to the tracking task that it is part or all of the solution for.

### Declarations

Check these if you believe they are true:

- [ ] The code base is in a better state after this PR
- [ ] Is documented according to the [standards](https://github.com/DynamoDS/Dynamo/wiki/Coding-Standards)
- [ ] The level of testing this PR includes is appropriate
- [ ] Snapshot of UI changes, if any

### Notes

(FILL ME IN, Optional) Anything else that you think is worth highlighting.
```

Unless otherwise noted, the Camber source files are distributed under the BSD 3-Clause license.

Contribution "Bar"
------------------

Changes will be merged if they make it easier for people to use Camber.

Changes will not be merged if they have narrowly-defined benefits. Contributions should also satisfy the other published guidelines defined in this document.

DOs and DON'Ts
--------------

Please do:

* **DO** follow the Dynamo [coding standards](https://github.com/DynamoDS/Dynamo/wiki/Coding-Standards) and [naming standards](https://github.com/DynamoDS/Dynamo/wiki/Naming-Standards)
* **DO** test your code. Automated testing is challenging in Dynamo, and so manual testing will suffice. Please include test graphs when adding new features. When fixing bugs, start with adding a test graph and summary that highlights how the current behavior is broken.
* **DO** keep discussions focused. When a new or related topic comes up it's often better to create a new issue than to side-track a discussion.

Please do not:

* **DON'T** submit big pull requests. Instead, file an issue and start a discussion so we can agree on a direction before you invest a large amount of time.
* **DON'T** commit code that you didn't write. If you find code that you think is a good fit to add to Camber, file an issue and start a discussion before proceeding.
* **DON'T** submit PRs that alter licensing related files or headers. If you believe there's a problem with them, file an issue and we can discuss it.

Commit Messages
---------------

Please format commit messages as follows (based on [A Note About Git Commit Messages](http://tbaggery.com/2008/04/19/a-note-about-git-commit-messages.html)):

```
Summarize change in 50 characters or less

Provide more detail after the first line. Leave one blank line below the
summary and wrap all lines at 72 characters or less.

If the change fixes an issue, leave another blank line after the final
paragraph and indicate which issue is fixed in the specific format
below.

Fix #42
```

Also do your best to factor commits appropriately, not too large with unrelated things in the same commit, and not too small with the same small change applied N times in N different commits.

Notes
---------------
This guide was based off of [DotNet Core Contributing Guide](https://github.com/dotnet/coreclr/blob/master/Documentation/project-docs/contributing.md).
