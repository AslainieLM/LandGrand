# 01: Rules core and star rule

**What to build:** Set up the game's rules core: a plain C# assembly with no dependency on Unity's engine types, plus an EditMode test assembly. Its first rule gives the stars for a cleared level from collected and total collectibles: 0–40% gives 1 star, 41–99% gives 2, 100% gives 3. A developer can open Unity's Test Runner and see the star tests pass.

**Blocked by:** None (can start immediately)

**Status:** done, committed by the user

- [ ] Rules core and test assemblies exist and compile in the Unity Editor alongside the existing scripts
- [ ] Star rule: 0/5, 1/5 and 2/5 give 1; 3/5 and 4/5 give 2; 5/5 gives 3
- [ ] Star rule: 40% gives 1, 41% gives 2, 99% gives 2, 100% gives 3 on larger totals
- [ ] A cleared level never gets fewer than 1 star
- [ ] All tests pass in the Test Runner (EditMode)
- [ ] Every new line of code has a short basic-English comment (D-033)
- [ ] Behaviour matches the spec and the final manuscript (D-036); anything new is raised with the user first

Spec: `../spec.md`. Design source: vault `Project/Gameplay Design.md`.
