using System;

using UnityEngine;

using Random = System.Random;


/// <summary>
/// Based on https://stackoverflow.com/questions/5817490/implementing-box-mueller-random-number-generator-in-c-sharp.
/// </summary>
public sealed class Stochast {
	private Random random;


	private bool phase;

	private float u;
	private float v;

	private float factor;


	public Stochast(int seed) {
		this.random = new Random(seed);

		phase = false;
	}

	public Stochast() : this(Environment.TickCount) {
		// Overload like Random does.
	}


	public float nextUniform() {
		return (float) random.NextDouble();
	}

	public float nextUniform(float minimum, float maximum) {
		float value = (float) random.NextDouble();

		return (1.0f - value) * minimum + value * maximum;
	}

	
	public float nextGaussian() {
		float z;

		if(phase) {
			z = v * factor;
		}
		else {
			float s;

			do {
				u = 2.0f * nextUniform() - 1.0f;
				v = 2.0f * nextUniform() - 1.0f;

				s = u * u + v * v;
			}
			while(s >= 1.0f);

			factor = Mathf.Sqrt(-2.0f * Mathf.Log(s) / s);

			z = u * factor;
		}

		phase = !phase;

		return z;
	}

	public float nextGaussian(float mean, float deviation) {
		return mean + nextGaussian() * deviation;
	}
}
