﻿namespace Common.Model.Actors;

public record Actor(
	Id<Actor> Id,
	string Name,
	DateTime CreatedOn
);

