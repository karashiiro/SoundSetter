import { program } from "commander";
import fs from "fs";

const offsetsPath = "../../SoundSetter/Offsets.json";

program
    .name("update-offsets")
    .description("A simple script to update the option offsets for SoundSetter.")
    .version("1.0.0");

program
    .command("increment")
    .description("Increment all offsets by the specified amount.")
    .argument("<amount>", "The amount to increment the offsets by.")
    .action((amount) => {
        const incrementAmount = parseInt(amount);
        if (isNaN(incrementAmount)) {
            throw new Error("amount was NaN.");
        }

        const offsetsRaw = fs.readFileSync(offsetsPath, { encoding: "utf-8" });
        const offsets = JSON.parse(offsetsRaw.trim());

        const updatedOffsets = Object.keys(offsets)
            .reduce((newOffsets, optionName) => {
                newOffsets[optionName] = offsets[optionName] + incrementAmount;
                return newOffsets;
            }, {});

        const serialized = JSON.stringify(updatedOffsets, undefined, 2);
        fs.writeFileSync(offsetsPath, serialized);
    });

program.parse();
