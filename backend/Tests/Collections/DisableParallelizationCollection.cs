namespace Apta.Tests.Collections;

// prevents tests from running in parallel because they
// share infrastructure resources such as the postgres database

[CollectionDefinition(nameof(DisableParallelizationCollection), DisableParallelization = true)]
public sealed class DisableParallelizationCollection;